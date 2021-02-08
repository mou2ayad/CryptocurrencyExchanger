using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Components.Utilities.Swagger;
using App.Components.Utilities.DependencyInjection;

using App.Components.Utilities.ErrorHandling;
using App.Services.CryptocurrencyExchangerAPI.DependencyInjection;
using Microsoft.Extensions.Logging;
using App.Components.Utilities.JWT_Auth;
using App.Services.CryptocurrencyExchangerAPI.Services;

namespace CryptocurrencyExchanger
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string API_NAME = "Cryptocurrency";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddOptions();
            services.AddHealthChecks();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMemoryCache();
            services.AddOptions();
            services.InjectCryptocurrencyProviderService(Configuration);
            services.InjectSwaggerServices(API_NAME, Configuration);
            services.AddSingleton<IUserService, StaticUserService>(); // this is just dev not for production
            services.InjectJWTService(Configuration);

          
        }
      
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILogger<Startup> log, IWebHostEnvironment env,IConfiguration configuration)
        {            
            app.UseSwaggerMiddleware(API_NAME, configuration);
            app.ConfigurErrorHandler(log, API_NAME, env.IsDevelopment());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseJWTMiddleware();
            app.UseHealthChecks(new Microsoft.AspNetCore.Http.PathString("/healthcheck"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
