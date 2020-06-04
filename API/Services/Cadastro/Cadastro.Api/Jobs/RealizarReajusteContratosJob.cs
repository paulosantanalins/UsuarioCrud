using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class RealizarReajusteContratosJob
    {
        public void Execute()
        {
            //var _variables = RecuperarVariablesToken();
            var _logGenericoRepository = RecuperarLogGenericoRepository();
            _logGenericoRepository.AddLogJob("Tentando rodar o JOB de efetivar Reajustes de Contratos.");

            if (!_logGenericoRepository.LogAdicionadoNoDiaAtual(
                "Inicio do JOB de efetivar Reajustes de Contratos"))
            {
                _logGenericoRepository.AddLogJob("Inicio do JOB de efetivar Reajustes de Contratos");

                //_variables.UserName = "STFCORP";
                var _reajusteContratoService = RecuperarReajusteContratoService();
                _reajusteContratoService.EfetuarReajustesDeContratos();
            }

            _logGenericoRepository.AddLogJob("Conclusão do JOB de Reajustes de Contratos");
        }

        private static IReajusteContratoService RecuperarReajusteContratoService()
        {
            var transfService =
                Injector.ServiceProvider.GetService(typeof(IReajusteContratoService)) as IReajusteContratoService;
            return transfService;
        }

        private static ILogGenericoRepository RecuperarLogGenericoRepository()
        {
            var logGenericoRepository =
                Injector.ServiceProvider.GetService(typeof(ILogGenericoRepository)) as ILogGenericoRepository;
            return logGenericoRepository;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
