using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Repository;
using System.Collections.Generic;
using System.Linq;

namespace RepasseEAcesso.Domain.RepasseRoot.Repository
{
    public interface IRepasseRepository : IBaseRepository<Repasse>
    {

        List<Repasse> BuscarTodosMigracaoRepasse();

        List<Repasse> BuscarTodosFilhosLegado(int idRepasseEacesso);
    }
}
