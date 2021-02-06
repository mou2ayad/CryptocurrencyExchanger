using Microsoft.Extensions.DependencyInjection;
using App.Components.Utilities.APIClient;



namespace App.Components.Utilities.DependencyInjection
{
    public static class RestAPIInjectionExtension
    {
        public static void InjectRestAPIServices(this IServiceCollection services) =>
            services.AddSingleton<IRestAPICaller, RestAPICaller>();

    }
}
