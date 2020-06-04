using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IValorPrestadorRepository : IBaseRepository<ValorPrestador>
    {
        List<ValorPrestador> BuscarComIncludePrestador();

        List<ValorPrestador> BuscarTodosPorIdPrestador(int idPrestador);
    }
}
