using System.Linq;
using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.BroadcastRoot.Repository;
using ControleAcesso.Domain.SharedRoot;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class BroadcastItemRepository : BaseRepository<BroadcastItem>, IBroadcastItemRepository
    {
        public BroadcastItemRepository(ControleAcessoContext context, IVariablesToken variables) : base(context, variables)
        {
        }

        public int BuscarUltimoId()
        {
            var ultimo = DbSet.LastOrDefault();

            return ultimo?.Id ?? 0;
        }
    }
}
