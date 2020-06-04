using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Infra.Data.SqlServer.Context;
using Cliente.Infra.Data.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Cliente.Domain.SharedRoot;
using Utils;

namespace Cliente.Infra.Data.SqlServer.Repository
{
    public class PaisRepository : BaseRepository<Pais>, IPaisRepository
    {
        protected readonly IVariablesToken _variables;
        public PaisRepository(ClienteContext clienteContext, IVariablesToken variables) : base(clienteContext, variables)
        {
            _variables = variables;
        }


        public void SalvarPaises(List<Pais> paises)
        {
            DbSet.AddRange(paises);
            _variables.UserName = "salesForce";
            _context.SaveChanges();
        }
    }
}
