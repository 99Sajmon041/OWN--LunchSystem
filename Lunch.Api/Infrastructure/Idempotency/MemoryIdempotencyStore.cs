
using Microsoft.Extensions.Caching.Memory;

namespace Lunch.Api.Infrastructure.Idempotency
{
    public sealed class MemoryIdempotencyStore : IIdempotencyStore
    {
        private readonly IMemoryCache _cache;
        public MemoryIdempotencyStore(IMemoryCache cache) => _cache = cache;

        public Task<bool> ExistsAsync(string key, CancellationToken ct)
            => Task.FromResult(_cache.TryGetValue(key, out _));

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct)
        {
            _cache.Set(key, value!, ttl);
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken ct)
            => Task.FromResult(_cache.TryGetValue(key, out T? v) ? v : default);
    }
}
