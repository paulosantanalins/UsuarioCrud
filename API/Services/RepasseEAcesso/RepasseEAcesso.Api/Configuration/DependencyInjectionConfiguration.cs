using Microsoft.Extensions.DependencyInjection;
using RepasseEAcesso.Infra.CrossCutting.IoC;

namespace RepasseEAcesso.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
           Injector.RegisterServices(services);
        }
    }
}
