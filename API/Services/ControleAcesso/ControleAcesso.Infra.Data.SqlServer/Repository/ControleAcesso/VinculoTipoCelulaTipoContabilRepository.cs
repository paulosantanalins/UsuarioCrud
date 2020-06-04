using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class VinculoTipoCelulaTipoContabilRepository : BaseRepository<VinculoTipoCelulaTipoContabil>, IVinculoTipoCelulaTipoContabilRepository
    {
        public VinculoTipoCelulaTipoContabilRepository(ControleAcessoContext context, IVariablesToken variables) 
            : base(context, variables)
        {
        }

        public List<VinculoTipoCelulaTipoContabil> BuscarTiposCelulas()
        {
            var result = DbSet.AsQueryable().Include(x=>x.Tipocelula).Include(x=>x.TipoContabil).ToList();
            return result;
        }
    }

}

