using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace RepasseEAcesso.Infra.Data.SqlServer.Repository.RepasseRepository
{    
    public class ProfissionaisRepository : BaseLegadoRepository<Profissionais>, IProfissionaisRepository
    {
        public ProfissionaisRepository(RepasseLegadoContext context, IVariablesToken variables) : base(context, variables)
        {

        }
    }
}
