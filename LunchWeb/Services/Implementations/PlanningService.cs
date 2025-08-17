using Lunch.Shared.Policies;
using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Interfaces;

namespace LunchWeb.Services.Implementations
{
    public class PlanningService : IPlanningService
    {
        private readonly IDailyOfferRepository _dailyOfferRepository;

        public PlanningService(IDailyOfferRepository dailyOfferRepository) => _dailyOfferRepository = dailyOfferRepository;
        public async Task<(bool show, string message)> CheckNextNextWeekAsync(DateOnly today, CancellationToken ct)
        {
            var mondayThisWeek = WeekEditPolicy.GetMondayOfWeek(today);
            var weekStart = mondayThisWeek.AddDays(14);
            var weekEnd = weekStart.AddDays(6);
            const int PositionsPerDay = 3;
            const int DaysPerWeek = 7;
            int expected = PositionsPerDay * DaysPerWeek;

            var count = await _dailyOfferRepository.CountOffersAsync(weekStart, weekEnd, ct);

            if (count != expected)
            {
                var msg = $"Nejsou naplánované obědy na týden: {weekStart:dd.MM.yyyy} - {weekEnd:dd.MM.yyyy}, aktuálně chybí doplánovat: {expected - count} !";
                return (true, msg);
            }
            return (false, "");
        }
    }
}
