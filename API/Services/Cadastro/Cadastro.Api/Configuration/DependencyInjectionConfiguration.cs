using Microsoft.Extensions.DependencyInjection;
using Cadastro.Infra.CrossCutting.IoC;

namespace Cadastro.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
           Injector.RegisterServices(services);
        }
    }
}
