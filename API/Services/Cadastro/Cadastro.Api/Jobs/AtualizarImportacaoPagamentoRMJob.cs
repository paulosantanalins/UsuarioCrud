using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class AtualizarImportacaoPagamentoRMJob
    {
        public void Execute()
        {
            var _pluginRMService = RecuperarPluginRMService();
            //var _variables = RecuperarVariablesToken();
            //_variables.UserName = "STFCORP";
            _pluginRMService.AtualizarSituacaoApartirDoRm();
        }

        private static IPluginRMService RecuperarPluginRMService()
        {
            var pluginRMService = Injector.ServiceProvider.GetService(typeof(IPluginRMService)) as IPluginRMService;
            return pluginRMService;
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
