namespace TaxLedgerAPI.Services
{
    public interface ICacheService
    {
        T? GetFromCacheByKey<T>(string key);

        T AddToCacheAndReturn<T>(string key, T objectToCache);
    }
}
