using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
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
    public class FuncionalidadeRepository : BaseRepository<Funcionalidade>, IFuncionalidadeRepository
    {
        public FuncionalidadeRepository(ControleAcessoContext controleAcessoContext, IVariablesToken variables) : base(controleAcessoContext, variables)
        {

        }

        public void BuscarUsers()
        {
            //var listaDoAndim = 
            //    DbSet.Include(x => x.VinculoPerfilFuncionalidades)
            //    .ThenInclude(x => x.Perfil)
            //    .ThenInclude(x => x.UsuarioPerfis)
            //    .ThenInclude(x => x.)
                
        }
    }
}
