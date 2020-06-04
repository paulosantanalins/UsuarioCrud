using GestaoServico.Domain.GestaoFilialRoot.Entity;
using GestaoServico.Domain.GestaoFilialRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPacoteServico
{
    public class EmpresaRepository : BaseRepository<Empresa>, IEmpresaRepository
    {

        public EmpresaRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {

        }
    }
}

