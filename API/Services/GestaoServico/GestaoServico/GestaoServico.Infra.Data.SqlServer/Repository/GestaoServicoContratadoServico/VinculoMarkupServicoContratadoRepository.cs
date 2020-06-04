using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoServicoContratadoServico
{
    public class VinculoMarkupServicoContratadoRepository : BaseRepository<VinculoMarkupServicoContratado>, IVinculoMarkupServicoContratadoRepository
    {
        public VinculoMarkupServicoContratadoRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
           : base(gestaoServicoContext, variables)
        {

        }
    }
}
