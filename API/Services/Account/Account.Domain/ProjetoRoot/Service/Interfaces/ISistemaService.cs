using Account.Domain.ProjetoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.ProjetoRoot.Service.Interfaces
{
    public interface ISistemaService
    {
        Task PersistirSistema(Sistema sistema);
    }
}
