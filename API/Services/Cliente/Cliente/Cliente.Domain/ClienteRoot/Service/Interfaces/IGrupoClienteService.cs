using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Service.Interfaces
{
    public interface IGrupoClienteService
    {
        int PersistirGrupoClienteSalesForce(string idSalesForceClienteMae, string descricaoGrupoCliente);
        int? ObterIdGrupoClientePorIdClienteMae(string idSalesForceClienteMae);
    }
}
