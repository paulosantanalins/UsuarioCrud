using Microsoft.Extensions.DependencyInjection;
using LancamentoFinanceiro.Infra.CrossCutting.IoC;

namespace LancamentoFinanceiro.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
           Injector.RegisterServices(services);
        }
    }
}
