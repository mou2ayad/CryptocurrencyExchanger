using Microsoft.AspNetCore.Builder;


namespace App.Components.Utilities.JWT_Auth
{
    public static class JWTMiddlewareExtension
    {
        public static void UseJWTMiddleware(this IApplicationBuilder app) =>
          app.UseMiddleware<JwtMiddleware>();
    }
}
