using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using System;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class SolicitarPagamentoPrestadorRMJob
    {
        public void Execute()
        {
            var _variables = RecuperarVariablesToken();
            _variables.UserName = "STFCORP";
            var _pluginRMService = RecuperarPluginRMService();
            var _horasMesPrestadorService = RecuperarHorasMesPrestadorService();
            var _horasMesRepository = RecuperarHorasMesRepository();
            var _logHorasMesPrestadorRepository = RecuperarLogHorasMesPrestadorRepository();

            var periodoVigente = _horasMesRepository.BuscarPeriodoVigente();
            if (periodoVigente != null)
            {
                var pagamentosPendentes = _horasMesPrestadorService.BuscarLancamentosComPagamentoPendente(periodoVigente.Id);
                var ontem = DateTime.Now.AddDays(-1).Date;
                var diaLimiteParaEnvioNfExpirado = periodoVigente.PeriodosDiaPagamento.FirstOrDefault(x => x.DiaLimiteEnvioNF.Date == ontem);

                if (diaLimiteParaEnvioNfExpirado != null)
                {
                    var lancamentosProntosParaRM = pagamentosPendentes.Where(x => x.Prestador.IdDiaPagamento == diaLimiteParaEnvioNfExpirado.IdDiaPagamento);
                    foreach (var lancamento in lancamentosProntosParaRM)
                    {
                        _pluginRMService.SolicitarPagamentoRM(lancamento.Id);
                    }
                }
            }
        }

        private static IPluginRMService RecuperarPluginRMService()
        {
            var pluginRMService = Injector.ServiceProvider.GetService(typeof(IPluginRMService)) as IPluginRMService;
            return pluginRMService;
        }

        private static IHorasMesPrestadorService RecuperarHorasMesPrestadorService()
        {
            var horasMesPrestadorService = Injector.ServiceProvider.GetService(typeof(IHorasMesPrestadorService)) as IHorasMesPrestadorService;
            return horasMesPrestadorService;
        }

        private static IHorasMesRepository RecuperarHorasMesRepository()
        {
            var horasMesRepository = Injector.ServiceProvider.GetService(typeof(IHorasMesRepository)) as IHorasMesRepository;
            return horasMesRepository;
        }

        private static ILogHorasMesPrestadorRepository RecuperarLogHorasMesPrestadorRepository()
        {
            var logHorasMesPrestadorRepository = Injector.ServiceProvider.GetService(typeof(ILogHorasMesPrestadorRepository)) as ILogHorasMesPrestadorRepository;
            return logHorasMesPrestadorRepository;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
