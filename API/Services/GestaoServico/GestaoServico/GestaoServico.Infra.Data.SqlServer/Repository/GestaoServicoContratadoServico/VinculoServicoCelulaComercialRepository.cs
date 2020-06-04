using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoServicoContratadoServico
{
    public class VinculoServicoCelulaComercialRepository : BaseRepository<VinculoServicoCelulaComercial>, IVinculoServicoCelulaComercialRepository
    {
        public VinculoServicoCelulaComercialRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {

        }
    }
}
