using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Infra.CrossCutting.IoC;
using Utils;

namespace ControleAcesso.Api.Jobs
{
    public class FinalizarInativacaoCelulasJob
    {
        public void Execute()
        {
            var _celulaService = RecuperarCelulaService();
            var _variables = RecuperarVariablesToken();
            _variables.UserName = "STFCORP";
            _celulaService.RealizarInativacoesJob();
        }

        private static ICelulaService RecuperarCelulaService()
        {
            var prestadorService = Injector.ServiceProvider.GetService(typeof(ICelulaService)) as ICelulaService;
            return prestadorService;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variables = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variables;
        }
    }
}
