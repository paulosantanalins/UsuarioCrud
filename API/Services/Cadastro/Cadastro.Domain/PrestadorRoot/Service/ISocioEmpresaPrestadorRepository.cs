using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public  interface ISocioEmpresaPrestadorRepository : IBaseRepository<SocioEmpresaPrestador>
    {
        SocioEmpresaPrestador BuscarPorIdEAcesso(int idEacesso);
        void UpdateRange(IEnumerable<SocioEmpresaPrestador> socios);
    }
}
