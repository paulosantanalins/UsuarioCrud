using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.Interfaces;

namespace ControleAcesso.Domain.BroadcastRoot.Repository
{
    public interface IBroadcastItemRepository : IBaseRepository<BroadcastItem>
    {
        int BuscarUltimoId();
    }
}
