using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class RealizarFinalizacaoContratosJob
    {
        public void Execute()
        {
            //var _variables = RecuperarVariablesToken();
            var _logGenericoRepository = RecuperarLogGenericoRepository();
            _logGenericoRepository.AddLogJob("Tentando rodar o JOB de efetivar finalização de Contrato de prestador.");


            if (!_logGenericoRepository.LogAdicionadoNoDiaAtual("Inicio do JOB de efetivar finalização de Contrato de prestador"))
            {
                _logGenericoRepository.AddLogJob("Inicio do JOB de efetivar finalização de Contrato de prestador");

                //_variables.UserName = "STFCORP";
                var _finalizacaoContratoService = RecuperarTransferenciaPrestadorService();
                _finalizacaoContratoService.EfetuarFinalizacoes();
            }

            _logGenericoRepository.AddLogJob("Conclusão do JOB de finalização de Contrato de prestador");
        }

        private static IFinalizacaoContratoService RecuperarTransferenciaPrestadorService()
        {
            var transfService = Injector.ServiceProvider.GetService(typeof(IFinalizacaoContratoService)) as IFinalizacaoContratoService;
            return transfService;
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
