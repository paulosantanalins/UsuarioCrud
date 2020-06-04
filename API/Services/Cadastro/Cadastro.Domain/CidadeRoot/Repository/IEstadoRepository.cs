using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.CidadeRoot.Repository
{
    public interface IEstadoRepository : IBaseRepository<Estado>
    {
        IEnumerable<Estado> BuscarEstadosBrasileiros();
    }
}
