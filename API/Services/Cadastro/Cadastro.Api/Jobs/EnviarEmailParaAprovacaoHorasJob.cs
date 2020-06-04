using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Api.Jobs
{
    public class EnviarEmailParaAprovacaoHorasJob
    {
        public void Execute()
        {
            //var _variables = RecuperarVariablesToken();
            //_variables.UserName = "STFCORP";
            var _horasMesPrestadorService = RecuperarHorasMesPrestadorService();
            _horasMesPrestadorService.EnviarEmailParaAprovacaoHoras();
        }

        private static IHorasMesPrestadorService RecuperarHorasMesPrestadorService()
        {
            var prestadorService = Injector.ServiceProvider.GetService(typeof(IHorasMesPrestadorService)) as IHorasMesPrestadorService;
            return prestadorService;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
