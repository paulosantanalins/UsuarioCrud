using EnvioEmail.Infra.CrossCutting.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace EnvioEmail.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
           Injector.RegisterServices(services);
        }
    }
}
