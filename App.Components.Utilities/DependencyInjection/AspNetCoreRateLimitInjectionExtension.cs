using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Components.Utilities.IpRateLimit;

namespace App.Components.Utilities.DependencyInjection
{
    public static class AspNetCoreRateLimitInjectionExtension
    {
        public static void InjectAspNetCoreRateLimitService(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimit"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
            services.AddHttpContextAccessor();
        }
       

    }

}
