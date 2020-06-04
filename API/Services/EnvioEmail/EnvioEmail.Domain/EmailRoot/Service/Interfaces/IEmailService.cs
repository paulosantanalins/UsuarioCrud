using EnvioEmail.Domain.EmailRoot.Entity;
using System.Collections.Generic;

namespace EnvioEmail.Domain.EmailRoot.Service.Interfaces
{
    public interface IEmailService
    {
        Email Enviar(Email email);
        void Reenviar(Email email);
        IEnumerable<Email> BuscarEmailsPendentes();
    }
}
