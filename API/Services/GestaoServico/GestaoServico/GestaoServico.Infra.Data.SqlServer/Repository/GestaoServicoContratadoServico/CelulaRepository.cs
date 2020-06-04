using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPacoteServico
{
    public class CelulaRepository : BaseRepository<Celula>, ICelulaRepository
    {
        public CelulaRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {

        }
    }
}
