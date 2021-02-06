using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace App.Components.Utilities.IpRateLimit
{
    public class IpX_FORWARDED_FOR_ResolveContributor : IIpResolveContributor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _headerName= "X-FORWARDED-FOR";

        public IpX_FORWARDED_FOR_ResolveContributor(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ResolveIp()
        {
            var httpContent = _httpContextAccessor.HttpContext;
            IPAddress clientIp=null;
            List<string> ips = ParseForwardedFor(httpContent.Request.Headers[_headerName]);
            if (ips.Count>0)            
                clientIp = IpAddressUtil.ParseIp(ips.First());                                            
            
            return clientIp?.ToString();
        }
        private List<string> ParseForwardedFor(string s)
        {
            List<string> a = new List<string>();
            if (string.IsNullOrEmpty(s))
                return a;
            string[] sa = s.Split(new[] { ',' });
            a.AddRange(sa.Select(z => z.Trim()));
            return a;
        }
    }
}