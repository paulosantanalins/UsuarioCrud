using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Repository
{
    public interface IGrupoClienteRepository : IBaseRepository<GrupoCliente>
    {
        int? ObterIdGrupoClientePorIdClienteMae(string idSalesForce);
    }
}
