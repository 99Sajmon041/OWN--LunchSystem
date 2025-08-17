namespace LunchWeb.Services.Interfaces
{
    public interface IPlanningService
    {
        Task<(bool show, string message)> CheckNextNextWeekAsync(DateOnly today, CancellationToken ct);
    }
}
