using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using System.Collections.Generic;

namespace ControleAcesso.Domain.BroadcastRoot.Repository
{
    public interface IBroadcastRepository : IBaseRepository<Broadcast>
    {
        IEnumerable<Broadcast> ObterBroadcastsDoUsuario(string usuario, string valorParaFiltrar);
        IEnumerable<Broadcast> ObterTodosBroadcastsNaoExcluidos(string usuario);
    }
}
