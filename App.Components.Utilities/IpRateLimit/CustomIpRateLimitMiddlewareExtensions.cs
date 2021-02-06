using Microsoft.AspNetCore.Builder;


namespace App.Components.Utilities.IpRateLimit
{
    public static class CustomIpRateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseIpRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomIpRateLimitMiddleware>();
        }
    }
}
