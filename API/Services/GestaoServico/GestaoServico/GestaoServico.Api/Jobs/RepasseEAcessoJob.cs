using AutoMapper;
using FluentScheduler;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Infra.CrossCutting.IoC;
using System.Collections.Generic;

namespace GestaoServico.Api.Jobs
{
    public class RepasseEAcessoJob : IJob
    {
        public void Execute()
        {
            var _repasseMigracaoService = RecuperarRepasseMigracaoService();

            var repassesEacesso = _repasseMigracaoService.BuscarRepassesEAcesso("01-01-2019", "30-06-2019");
            var repasses = Mapper.Map<List<Repasse>>(repassesEacesso);

            _repasseMigracaoService.MigrarRepassesEacesso(repasses);
        }
        
        private static IRepasseMigracaoService RecuperarRepasseMigracaoService()
        {
            var repasseMigracaoService = Injector.ServiceProvider.GetService(typeof(IRepasseMigracaoService)) as IRepasseMigracaoService;
            return repasseMigracaoService;
        }
    }
}
