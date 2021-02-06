using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using App.Components.Utilities.Logger;

namespace App.Components.Utilities.IpRateLimit
{
    public class CustomIpRateLimitMiddleware : RateLimitMiddleware<IpRateLimitProcessor>
    {
        private readonly ILogger<IpRateLimitMiddleware> _logger;

        public CustomIpRateLimitMiddleware(RequestDelegate next,
            IOptions<IpRateLimitOptions> options,
            IRateLimitCounterStore counterStore,
            IIpPolicyStore policyStore,
            IRateLimitConfiguration config,
            ILogger<IpRateLimitMiddleware> logger)
        : base(next, options?.Value, new IpRateLimitProcessor(options?.Value, counterStore, policyStore, config), config)
        {
            _logger = logger;
   
        }

        protected override void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            _logger.LogInfoDetails("IpRateLimit=> Request {0}:{1} from IP {2} has been blocked, quota {3}/{4} exceeded by {5}. Blocked by rule {6}, TraceIdentifier {7}.", identity.HttpVerb, identity.Path, identity.ClientIp, rule.Limit, rule.Period, counter.Count, rule.Endpoint, httpContext.TraceIdentifier);
        }
    }

}
