using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace App.Components.Utilities.IpRateLimit
{
    public class CustomRateLimitConfiguration : RateLimitConfiguration
    {
        
        public CustomRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions) :base(httpContextAccessor, ipOptions, clientOptions)
        {
            
        }
        protected override void RegisterResolvers()
        {
            IpResolvers.Add(new IpX_FORWARDED_FOR_ResolveContributor(HttpContextAccessor));
            IpResolvers.Add(new IpConnectionResolveContributor(HttpContextAccessor));
        }
    }
}