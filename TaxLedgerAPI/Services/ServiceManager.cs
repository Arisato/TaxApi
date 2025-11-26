namespace TaxLedgerAPI.Services
{
    public static class ServiceManager
    {
        public static IServiceCollection AddLocalServices(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IBracketService, BracketService>();
            services.AddScoped<ILedgerService, LedgerService>();
            services.AddScoped<IMunicipalityService, MunicipalityService>();

            return services;
        }
    }
}
