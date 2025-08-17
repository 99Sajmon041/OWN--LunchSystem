using Lunch.Domain.Models;
using Lunch.Shared.Policies;
using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.DailyOfferViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Services.Implementations
{
    public class DailyOfferService : IDailyOfferService
    {
        private readonly IDailyOfferRepository _dailyOfferRepository;
        private readonly IMealRepository _mealRepository;

        public DailyOfferService(IDailyOfferRepository dailyOfferRepository, IMealRepository mealRepository)
        {
            _dailyOfferRepository = dailyOfferRepository;
            _mealRepository = mealRepository;
        }

        public async Task<DailyOfferWeeklyViewModel> GetWeeklyOfferAsync(DateOnly weekStart)
        {
            weekStart = WeekEditPolicy.NormalizeToMonday(weekStart);
            var today = DateOnly.FromDateTime(DateTime.Today);

            var allMeals = await _mealRepository.GetAllMealsAsync();
            if (allMeals == null || !allMeals.Any())
                throw new InvalidOperationException("Nebyla nalezena žádná jídla v systému.");

            var endExclusive = weekStart.AddDays(7);

            var offers = await _dailyOfferRepository.GetAll()
                .Where(x => x.Date >= weekStart && x.Date < endExclusive)
                .Include(x => x.Meal)
                .AsNoTracking()
                .ToListAsync();

            var byDatePos = offers
                .GroupBy(o => new { o.Date, o.Position })
                .ToDictionary(g => (g.Key.Date, g.Key.Position), g => g.First());

            var vm = new DailyOfferWeeklyViewModel
            {
                StartOfWeek = weekStart,
                CanEditEntireWeek = WeekEditPolicy.CanEditWeek(weekStart, today),
                LockedWeekStart = WeekEditPolicy.GetLockedWeekStart(today),
                FirstEditableWeekStart = WeekEditPolicy.GetFirstEditableWeekStart(today),
                CanGoBack = weekStart > WeekEditPolicy.GetLaunchWeekStart(),
                IsCurrentWeek = weekStart == WeekEditPolicy.NormalizeToMonday(today),
                ItemsPerDay = new List<DailyOfferDayItem>()
            };

            for (int i = 0; i < 7; i++)
            {
                var date = weekStart.AddDays(i);
                var day = new DailyOfferDayItem { Date = date };

                for (int pos = 0; pos < 3; pos++)
                {
                    byDatePos.TryGetValue((date, pos), out var existing);
                    day.Meals.Add(new DailyMealItem
                    {
                        MealId = existing?.MealId,
                        MealOptions = new SelectList(allMeals, "Id", "FoodName", existing?.MealId)
                    });
                }

                vm.ItemsPerDay.Add(day);
            }

            return vm;
        }


        public async Task SaveWeeklyOfferAsync(DailyOfferWeeklyViewModel vm)
        {
            foreach (var dayItem in vm.ItemsPerDay)
            {
                var duplicates = dayItem.Meals
                    .Where(m => m.MealId != null)
                    .GroupBy(m => m.MealId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicates.Any())
                {
                    throw new InvalidOperationException(
                        $"Ve dni {dayItem.Date} je vybrané stejné jídlo vícekrát (MealId: {string.Join(",", duplicates)}).");
                }
            }
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            var weekStart = WeekEditPolicy.NormalizeToMonday(vm.StartOfWeek);
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (!WeekEditPolicy.CanEditWeek(weekStart, today))
                throw new InvalidOperationException("Tento týden nelze upravit. Zamčený je vždy bezprostředně následující týden.");

            var endExclusive = weekStart.AddDays(7);

            var existingOffers = await _dailyOfferRepository.GetAll()
                .Where(x => x.Date >= weekStart && x.Date < endExclusive)
                .ToListAsync();

            var existingByDatePos = existingOffers
                .GroupBy(x => new { x.Date, x.Position })
                .ToDictionary(g => (g.Key.Date, g.Key.Position), g => g.OrderBy(o => o.Id).First());

            var toCreate = new List<DailyOffer>();
            var toDelete = new List<DailyOffer>();
            var toUpdate = new List<DailyOffer>();

            const int positionsPerDay = 3;

            foreach (var dayItem in vm.ItemsPerDay)
            {
                for (int pos = 0; pos < Math.Min(positionsPerDay, dayItem.Meals.Count); pos++)
                {
                    var selectedMealId = dayItem.Meals[pos].MealId;
                    existingByDatePos.TryGetValue((dayItem.Date, pos), out var existing);

                    if (selectedMealId == null)
                    {
                        if (existing != null) toDelete.Add(existing);
                        continue;
                    }

                    if (existing != null)
                    {
                        if (existing.MealId != selectedMealId.Value)
                        {
                            existing.MealId = selectedMealId.Value;
                            toUpdate.Add(existing);
                        }
                    }
                    else
                    {
                        toCreate.Add(new DailyOffer
                        {
                            Date = dayItem.Date,
                            Position = pos,
                            MealId = selectedMealId.Value
                        });
                    }
                }
            }

            if (toUpdate.Any())
                await _dailyOfferRepository.UpdateRangeAsync(toUpdate);

            if (toCreate.Any())
                await _dailyOfferRepository.CreateRangeAsync(toCreate);

            if (toDelete.Any())
                await _dailyOfferRepository.RemoveRangeAsync(toDelete);
        }
    }
}
