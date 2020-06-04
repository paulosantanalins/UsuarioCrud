using EnvioEmail.Domain.EmailRoot.Service.Interfaces;
using EnvioEmail.Infra.CrossCutting.IoC;
using Logger.Repository.Interfaces;
using System.Linq;

namespace EnvioEmail.Api.Jobs
{
    public class EnviarEmailComErro
    {
        public void Execute()
        {
            var _emailService = RecuperarIEmailService();
            var emails = _emailService.BuscarEmailsPendentes().ToList();
            foreach (var email in emails)
            {
                _emailService.Reenviar(email);
            }
        }

        private static IEmailService RecuperarIEmailService()
        {
            var importacaoService = Injector.ServiceProvider.GetService(typeof(IEmailService)) as IEmailService;
            return importacaoService;
        }
    }
}
