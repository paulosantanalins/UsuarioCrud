using Microsoft.Extensions.DependencyInjection;
using Forecast.Infra.CrossCutting.IoC;

namespace Forecast.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
           Injector.RegisterServices(services);
        }
    }
}
