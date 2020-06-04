using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces;
using RepasseEAcesso.Infra.CrossCutting.IoC;
using System;

namespace RepasseEAcesso.Api.Jobs
{
    public class AtualizarPeriodoEAcessoJob
    {
        public void Execute()
        {
            var _periodoRepasseService = RecuperarPeriodoRepasseService();
            var periodoVigente = _periodoRepasseService.BuscarPeriodoVigente();
            var hoje = DateTime.Now.Date;

            if (hoje > periodoVigente.DtLancamentoFim)
            {
                _periodoRepasseService.PopularDataFimLancamentoEacesso(periodoVigente);
            }

            if (hoje > periodoVigente.DtAprovacaoFim)
            {
                _periodoRepasseService.PopularDataFimEacesso(periodoVigente);
            }
        }

        private static IPeriodoRepasseService RecuperarPeriodoRepasseService()
        {
            var periodoRepasseService = Injector.ServiceProvider.GetService(typeof(IPeriodoRepasseService)) as IPeriodoRepasseService;
            return periodoRepasseService;
        }
    }
}
