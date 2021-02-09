using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Components.Utilities.JWT_Auth
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IOptions<JWTSettings> jwtSettings, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var splits = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ');
            if(splits!=null && splits.Length==2 && splits[0].ToLower()=="bearer" && splits[1] != null)
                AttachUserToContext(context, splits[1]);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var UserName = jwtToken.Claims.First(x => x.Type == "UserName").Value;
                var userpermissions = jwtToken.Claims.First(x => x.Type == "permissions").Value;
                var permissions = new List<string>();
                if (!string.IsNullOrEmpty(userpermissions))
                    permissions.AddRange(userpermissions.Split(','));
                context.Items["UserClient"] = new UserClient() { UserName = UserName, Permissions = permissions };
            }
            catch
            {
                _logger.LogInformation("unauthorized attempt to call the api");
            }
        }
    }
}
