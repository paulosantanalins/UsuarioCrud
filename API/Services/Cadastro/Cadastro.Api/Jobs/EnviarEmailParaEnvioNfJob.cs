using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using System;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class EnviarEmailParaEnvioNfJob
    {
        public void Execute()
        {
            //var _variables = RecuperarVariablesToken();
            //_variables.UserName = "STFCORP";
            var _prestadorService = RecuperarPrestadorService();
            var _horasMesPrestadorService = RecuperarHorasMesPrestadorService();
            var _horasMesRepository = RecuperarHorasMesRepository();

            var lancamentosAprovados = _horasMesPrestadorService.BuscarLancamentosAprovados();
            var amanha = DateTime.Now.AddDays(1).Date;
            var diaLimiteParaEnvioNfExpirado = _horasMesRepository.BuscarPeriodoVigente().PeriodosDiaPagamento.FirstOrDefault(x =>
                                                    x.DiaLimiteEnvioNF.Date == DateTime.Now.Date || x.DiaLimiteEnvioNF.Date == amanha);

            if (diaLimiteParaEnvioNfExpirado != null)
            {
                var lancamentosAprovadosParaDiaLimite = lancamentosAprovados.Where(x => x.Prestador.IdDiaPagamento == diaLimiteParaEnvioNfExpirado.IdDiaPagamento);
                foreach (var lancamento in lancamentosAprovadosParaDiaLimite)
                {
                    _prestadorService.SolicitarNF(lancamento.Id);
                }
            }
        }

        private static IPrestadorService RecuperarPrestadorService()
        {
            var prestadorService = Injector.ServiceProvider.GetService(typeof(IPrestadorService)) as IPrestadorService;
            return prestadorService;
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

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}