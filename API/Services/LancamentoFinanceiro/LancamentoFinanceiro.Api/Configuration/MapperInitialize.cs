using AutoMapper;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace LancamentoFinanceiro.Api.Configuration
{
    public static class MapperInitialize
    {
        public static void AddMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DtoToDomainMapping());
                //cfg.AddProfile(new DomainToDtoMapping());
            });

            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
