using EnvioEmail.Domain.EmailRoot.Entity;
using System.Collections.Generic;

namespace EnvioEmail.Domain.EmailRoot.Repository
{
    public interface IEmailRepository : IBaseRepository<Email>
    {
        IEnumerable<Email> BuscarEmailsPendentes();
    }
}
