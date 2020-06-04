using Account.Domain.ProjetoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.ProjetoRoot.Repository
{
    public interface ISistemaRepository
    {
        Task AddSistema(Sistema sistema);
        Task UpdateSistema(Sistema sistema);
        Task<List<Sistema>> ObterTodosSistemas();
        Task DeleteSistema(int id);
    }
}
