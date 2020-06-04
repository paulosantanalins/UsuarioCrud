using AutoMapper;
using FluentScheduler;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface;
using LancamentoFinanceiro.Infra.CrossCutting.IoC;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace LancamentoFinanceiro.Api.Job
{
    public class LancamentoFinanceiroJob : IJob
    {
        public void Execute()
        {
            var _migracaoRMService = RecuperarRepasseMigracaoService();

            var lancamentos = _migracaoRMService.BuscarLancamentosFinanceirosRM("01-01-2019", "31-01-2019");
            var rootLancamentos = Mapper.Map<List<RootLancamentoFinanceiro>>(lancamentos);

            var vezes = 0;
            const int take = 50;
            var lancamentosTake = rootLancamentos.Skip(0).Take(take).ToList();
            while (lancamentosTake.Any())
            {
                _migracaoRMService.MigrarLancamentosFinanceirosRM(lancamentosTake);
                vezes++;
                lancamentosTake = rootLancamentos.Skip(vezes * take).Take(take).ToList();
            }
        }

        private static IMigracaoRMService RecuperarRepasseMigracaoService()
        {
            var migracaoRMService = Injector.ServiceProvider.GetServices(typeof(IMigracaoRMService)).FirstOrDefault() as IMigracaoRMService;
            return migracaoRMService;
        }
    }
}
