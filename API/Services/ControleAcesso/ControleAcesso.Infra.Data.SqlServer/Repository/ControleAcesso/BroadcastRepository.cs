using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.BroadcastRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class BroadcastRepository : BaseRepository<Broadcast>, IBroadcastRepository
    {
        public BroadcastRepository(ControleAcessoContext context, IVariablesToken variables) : base(context, variables)
        {
        }

        public IEnumerable<Broadcast> ObterBroadcastsDoUsuario(string usuario, string valorParaFiltrar) =>
            DbSet.Include(x => x.BroadcastItem)
            .Where(x => valorParaFiltrar == "LIDOS" ? x.Lido && !x.Excluido && x.LgUsuarioVinculado == usuario
            : valorParaFiltrar == "NAOLIDOS" ? !x.Lido && !x.Excluido && x.LgUsuarioVinculado == usuario 
            : valorParaFiltrar == "EXCLUIDOS" ? x.Excluido == true && x.LgUsuarioVinculado == usuario
            : x.LgUsuarioVinculado == usuario)
            .OrderByDescending(x => x.DataCriacao)
            .ToList();

        public IEnumerable<Broadcast> ObterTodosBroadcastsNaoExcluidos(string usuario) =>
            DbSet.Include(x => x.BroadcastItem)
            .Where(x => x.LgUsuarioVinculado == usuario && !x.Excluido)
            .OrderByDescending(x => x.Id)
            .ToList();



    }
}
