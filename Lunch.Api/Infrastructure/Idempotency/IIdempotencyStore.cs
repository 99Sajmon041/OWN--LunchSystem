namespace Lunch.Api.Infrastructure.Idempotency
{
    public interface IIdempotencyStore
    {
        Task<bool> ExistsAsync(string key, CancellationToken ct);
        Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct);
        Task<T?> GetAsync<T>(string key, CancellationToken ct);
    }
}
