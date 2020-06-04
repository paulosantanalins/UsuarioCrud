using Microsoft.Extensions.DependencyInjection;
using UsuarioApi.Infra.CrossCutting.IoC;


namespace UsuarioApi.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            Injector.RegisterServices(services);
        }
    }
}
