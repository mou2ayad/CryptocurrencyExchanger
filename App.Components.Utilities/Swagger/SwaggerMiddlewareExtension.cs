using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace App.Components.Utilities.Swagger
{
    public static class SwaggerMiddlewareExtension
    {
        public static void UseSwaggerMiddleware(this IApplicationBuilder app, string APIName, IConfiguration configuration)
        {          
                string Version = configuration.GetValue<string>("Swagger:SwaggerDocs:Version") ?? "v1";
                string SwaggerEndpointUrl = $"/swagger/{Version}/swagger.json";
                string RoutePrefix = configuration.GetValue<string>("Swagger:RoutePrefix") ?? "swagger";

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(SwaggerEndpointUrl, APIName);
                    c.RoutePrefix = RoutePrefix;
                });
            }      
    }
}
