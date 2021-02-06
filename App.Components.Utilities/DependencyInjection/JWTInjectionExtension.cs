using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Components.Utilities.JWT_Auth;
using Microsoft.AspNetCore.Builder;

namespace App.Components.Utilities.DependencyInjection
{
    public static class JWTInjectionExtension
    {
        public static void InjectJWTService(this IServiceCollection services,IConfiguration configuration) =>    
            services.Configure<JWTSettings>(configuration.GetSection("JWT"));           
        
      
        
       
    }

}
