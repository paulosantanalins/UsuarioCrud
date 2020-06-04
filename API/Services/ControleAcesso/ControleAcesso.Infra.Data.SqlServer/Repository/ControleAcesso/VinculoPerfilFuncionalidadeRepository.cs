using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class VinculoPerfilFuncionalidadeRepository : BaseRepository<VinculoPerfilFuncionalidade>, IVinculoPerfilFuncionalidadeRepository
    {
        public VinculoPerfilFuncionalidadeRepository(ControleAcessoContext controleAcessoContext, IVariablesToken variables) : base(controleAcessoContext, variables)
        {

        }


        public void RemoverVinculos(List<VinculoPerfilFuncionalidade> vinculoPerfils)
        {
            DbSet.RemoveRange(vinculoPerfils);
        }
    }
}
