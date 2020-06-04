using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class RealizarTransferenciaPrestadorJob
    {
        public void Execute()
        {
            //var _variables = RecuperarVariablesToken();
            var _logGenericoRepository = RecuperarLogGenericoRepository();
            _logGenericoRepository.AddLogJob("Tentando rodar o JOB de efetivar transferências de prestador.");


            if (!_logGenericoRepository.LogAdicionadoNoDiaAtual("Inicio do JOB de efetivar transferências de prestador"))
            {
                _logGenericoRepository.AddLogJob("Inicio do JOB  de efetivar transferências de prestador");

                //_variables.UserName = "STFCORP";
                var _transfService = RecuperarTransferenciaPrestadorService();
                _transfService.EfetivarTransferenciasDePrestadoresJob();
            }

            _logGenericoRepository.AddLogJob("Conclusão do JOB de solicitar aprovação de horas");
        }

        private static ITransferenciaPrestadorService RecuperarTransferenciaPrestadorService()
        {
            var transfService = Injector.ServiceProvider.GetService(typeof(ITransferenciaPrestadorService)) as ITransferenciaPrestadorService;
            return transfService;
        }

        private static ILogTransferenciaPrestadorRepository RecuperarLogTransferenciaPrestadorRepository()
        {
            var logRepository = Injector.ServiceProvider.GetService(typeof(ILogTransferenciaPrestadorRepository)) as ILogTransferenciaPrestadorRepository;
            return logRepository;
        }

        private static ILogGenericoRepository RecuperarLogGenericoRepository()
        {
            var logGenericoRepository = Injector.ServiceProvider.GetService(typeof(ILogGenericoRepository)) as ILogGenericoRepository;
            return logGenericoRepository;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
