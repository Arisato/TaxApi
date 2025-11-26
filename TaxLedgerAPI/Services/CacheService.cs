using Microsoft.Extensions.Caching.Memory;

namespace TaxLedgerAPI.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache cache;

        public CacheService(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public T? GetFromCacheByKey<T>(string key) 
        {
            cache.TryGetValue(key, out T? cachedObject);

            return cachedObject;
        }

        public T AddToCacheAndReturn<T>(string key, T objectToCache)
        {
            var options = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(3000) };

            cache.Set(key, objectToCache, options);

            return objectToCache;
        }
    }
}
