using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Infra.Data.SqlServer.Context;
using Cliente.Infra.Data.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cliente.Domain.SharedRoot;
using Utils;

namespace Cliente.Infra.Data.SqlServer.Repository
{
    public class GrupoClienteRepository : BaseRepository<GrupoCliente>, IGrupoClienteRepository
    {
        public GrupoClienteRepository(ClienteContext clienteContext, IVariablesToken variables) : base(clienteContext, variables)
        {          
        }

        public int? ObterIdGrupoClientePorIdClienteMae(string idSalesForce)
        {
            var query = DbSet.Where(x => x.IdClienteMae == idSalesForce);
            var result = query.Select(x => x.Id).FirstOrDefault();

            return result;

        }
    }
}
