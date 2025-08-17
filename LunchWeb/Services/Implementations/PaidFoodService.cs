using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using Lunch.Shared.Policies;
using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.PaidFoodViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Services.Implementations
{
    public class PaidFoodService : IPaidFoodService
    {
        private readonly IDailyOfferRepository _dailyOfferRepository;
        private readonly IPaidFoodRepository _paidFoodRepository;
        private readonly UserManager<Customer> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IDiscountService _discountService;

        public PaidFoodService(
            IDailyOfferRepository dailyOfferRepository,
            IPaidFoodRepository paidFoodRepository,
            UserManager<Customer> userManager,
            ApplicationDbContext context,
            IDiscountService discountService)
        {
            _dailyOfferRepository = dailyOfferRepository;
            _paidFoodRepository = paidFoodRepository;
            _userManager = userManager;
            _context = context;
            _discountService = discountService;
        }

        public async Task<PaidFoodWeeklyViewModel> GetWeeklyPaidFoodAsync(string customerId, DateOnly weekStart)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("Neplatné ID zákazníka.", nameof(customerId));

            weekStart = WeekEditPolicy.NormalizeToMonday(weekStart);
            var today = DateOnly.FromDateTime(DateTime.Today);

            var canEdit = WeekEditPolicy.CanEditWeek(weekStart, today);
            var lockedWeekStart = WeekEditPolicy.GetLockedWeekStart(today);
            var firstEditableWeekStart = WeekEditPolicy.GetFirstEditableWeekStart(today);

            var weekEndExclusive = weekStart.AddDays(7);

            var customer = await _userManager.FindByIdAsync(customerId)
                ?? throw new InvalidOperationException("Zákazník nenalezen.");

            var roles = await _userManager.GetRolesAsync(customer);
            var roleName = roles.FirstOrDefault() ?? "Zakaznik";
            var discount = await _discountService.GetByroleName(roleName);

            var offers = await _dailyOfferRepository.GetAll()
                .AsNoTracking()
                .Where(o => o.Date >= weekStart && o.Date < weekEndExclusive)
                .Include(o => o.Meal)
                .ToListAsync();

            var existing = await _paidFoodRepository.GetAll()
                .AsNoTracking()
                .Include(f => f.DailyOffer)
                .Where(p => p.CustomerId == customerId
                            && p.DailyOffer != null
                            && p.DailyOffer.Date >= weekStart
                            && p.DailyOffer.Date < weekEndExclusive)
                .ToListAsync();

            var offersByDate = offers
                .GroupBy(o => o.Date)
                .ToDictionary(g => g.Key, g => g.OrderBy(o => o.Position).ToList());

            var existingByDate = existing
                .GroupBy(p => p.DailyOffer!.Date)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Id).First().DailyOfferId);



            var vm = new PaidFoodWeeklyViewModel
            {
                StartOfWeek = weekStart,
                CanEdit = canEdit,
                LockedWeekStart = lockedWeekStart,
                FirstEditableWeekStart = firstEditableWeekStart,
                CurrentBalance = customer.Credit,
                CanGoBack = weekStart > WeekEditPolicy.GetLaunchWeekStart()
            };

            int totalAvailableOptions = 0;

            for (int i = 0; i < 7; i++)
            {
                var date = weekStart.AddDays(i);

                if (!offersByDate.TryGetValue(date, out var dayOffers))
                    dayOffers = new List<DailyOffer>();

                var mealsOptions = dayOffers
                    .Select(o => new MealOptionVm
                    {
                        DailyOfferId = o.Id,
                        FoodName = o.Meal.FoodName,
                        FinalPrice = o.Meal.Price,
                        FinalPriceAfterDiscount = decimal.Round(o.Meal.Price * (1 - discount), 2, MidpointRounding.AwayFromZero),
                        ImageUrl = o.Meal.ImageUrl
                    })
                    .ToList();

                var items = dayOffers
                    .Select(o =>
                    {
                        var finalPrice = decimal.Round(o.Meal.Price * (1 - discount), 2, MidpointRounding.AwayFromZero);
                        return new SelectListItem($"{o.Meal.FoodName} - {finalPrice:0.##} Kč", o.Id.ToString());
                    })
                    .ToList();

                totalAvailableOptions += items.Count;

                existingByDate.TryGetValue(date, out var preselectedId);
                int? preselected = preselectedId == 0 ? null : preselectedId;

                vm.ItemsPerDay.Add(new PaidFoodDayItem
                {
                    Date = date,
                    SelectedDailyOfferId = preselected,
                    Options = new SelectList(items, "Value", "Text", preselected?.ToString()),
                    MealOptions = mealsOptions
                });
            }

            vm.CanSave = canEdit && totalAvailableOptions > 0;
            return vm;
        }


        public async Task SaveWeeklyPaidFoodAsync(string customerId, PaidFoodWeeklyViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("Neplatné ID zákazníka.", nameof(customerId));

            var weekStart = WeekEditPolicy.NormalizeToMonday(viewModel.StartOfWeek);
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (!WeekEditPolicy.CanEditWeek(weekStart, today))
                throw new InvalidOperationException("Tento týden nelze objednat/upravit. Zamčený je vždy bezprostředně následující týden.");

            var weekEndExclusive = weekStart.AddDays(7);

            var customer = await _userManager.FindByIdAsync(customerId)
                ?? throw new InvalidOperationException("Zákazník nenalezen.");

            var roles = await _userManager.GetRolesAsync(customer);
            var roleName = roles.FirstOrDefault() ?? "Zakaznik";
            var discount = await _discountService.GetByroleName(roleName);

            var weekOffers = _dailyOfferRepository.GetAll()
                .AsNoTracking()
                .Where(o => o.Date >= weekStart && o.Date < weekEndExclusive)
                .Select(o => new { o.Id, o.Date, o.Meal.Price })
                .ToList();

            var offerDateById = weekOffers.ToDictionary(o => o.Id, o => o.Date);
            var offerPriceById = weekOffers.ToDictionary(o => o.Id, o => o.Price);
            var offerFinalPriceById = weekOffers.ToDictionary(
                o => o.Id,
                o => decimal.Round(o.Price * (1 - discount), 2, MidpointRounding.AwayFromZero)
            );

            foreach (var dayVm in viewModel.ItemsPerDay)
            {
                if (dayVm.SelectedDailyOfferId is int selId)
                {
                    if (!offerDateById.TryGetValue(selId, out var offerDate))
                        throw new InvalidOperationException($"Nabídka s ID {selId} neexistuje v daném týdnu.");
                    if (offerDate != dayVm.Date)
                        throw new InvalidOperationException($"Nabídka {selId} neodpovídá dni {dayVm.Date}.");
                }
            }

            var existing = _paidFoodRepository.GetAll()
                .Where(p => p.CustomerId == customerId
                            && p.DailyOffer.Date >= weekStart
                            && p.DailyOffer.Date < weekEndExclusive)
                .ToList();

            var existingByDate = existing
                .GroupBy(p => p.DailyOffer.Date)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Id).ToList());

            var finalSelectionByDate = new Dictionary<DateOnly, int?>();
            foreach (var d in viewModel.ItemsPerDay.OrderBy(d => d.Date))
                finalSelectionByDate[d.Date] = d.SelectedDailyOfferId;

            var alreadyPaidTotal = existing
                .Where(p => p.Paid)
                .GroupBy(p => p.DailyOffer.Date)
                .Select(g => g.First())
                .Sum(p => p.FinalPaidPrice);

            var finalOfferIds = finalSelectionByDate.Values
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            var finalTotal = finalOfferIds.Sum(id => offerFinalPriceById[id]);
            var delta = finalTotal - alreadyPaidTotal;

            if (delta > 0 && customer.Credit < delta)
                throw new InvalidOperationException($"Nedostatečný kredit. Potřebujete doplatit {delta:0.##} Kč, aktuální kredit: {customer.Credit:0.##} Kč.");

            var toCreate = new List<PaidFood>();
            var toUpdate = new List<PaidFood>();
            var toDelete = new List<PaidFood>();

            foreach (var dayVm in viewModel.ItemsPerDay.OrderBy(d => d.Date))
            {
                existingByDate.TryGetValue(dayVm.Date, out var existingList);
                var primary = existingList?.FirstOrDefault();

                if (existingList != null && existingList.Count > 1)
                    toDelete.AddRange(existingList.Skip(1));

                var finalId = finalSelectionByDate[dayVm.Date];
                if (finalId == null)
                {
                    if (primary != null) toDelete.Add(primary);
                    continue;
                }

                if (primary != null)
                {
                    if (primary.DailyOfferId != finalId.Value)
                        primary.DailyOfferId = finalId.Value;

                    primary.FinalPaidPrice = offerFinalPriceById[finalId.Value];

                    if (!primary.Paid)
                        primary.Paid = true;

                    toUpdate.Add(primary);
                }
                else
                {
                    toCreate.Add(new PaidFood
                    {
                        CustomerId = customerId,
                        DailyOfferId = finalId.Value,
                        Paid = true,
                        FinalPaidPrice = offerFinalPriceById[finalId.Value]
                    });
                }
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (toUpdate.Any())
                    await _paidFoodRepository.UpdateRangeAsync(toUpdate);

                if (toCreate.Any())
                    await _paidFoodRepository.CreateRangeAsync(toCreate);

                if (toDelete.Any())
                    await _paidFoodRepository.RemoveRangeAsync(toDelete);

                if (delta != 0)
                {
                    if (delta > 0) customer.Credit -= delta;
                    else customer.Credit += Math.Abs(delta);

                    var res = await _userManager.UpdateAsync(customer);
                    if (!res.Succeeded)
                        throw new InvalidOperationException("Nepodařilo se aktualizovat kredit zákazníka.");
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<MealOptionVm?> GetMealOptionAsync(int dailyOfferId, string customerId)
        {
            if (dailyOfferId <= 0)
                return null;

            var offer = await _dailyOfferRepository.GetAll()
                .AsNoTracking()
                .Include(o => o.Meal)
                .FirstOrDefaultAsync(o => o.Id == dailyOfferId);

            if (offer == null || offer.Meal == null)
                return null;

            var customer = await _userManager.FindByIdAsync(customerId);
            if (customer == null)
                return null;

            var role = await _userManager.GetRolesAsync(customer);
            var roleName = role.FirstOrDefault() ?? "Zakaznik";
            var discount = await _discountService.GetByroleName(roleName);


            return new MealOptionVm
            {
                DailyOfferId = offer.Id,
                FoodName = offer.Meal.FoodName,
                FinalPrice = offer.Meal.Price,
                FinalPriceAfterDiscount = decimal.Round(offer.Meal.Price * (1 - discount), 2, MidpointRounding.AwayFromZero),
                ImageUrl = offer.Meal.ImageUrl
            };
        }
    }
}
