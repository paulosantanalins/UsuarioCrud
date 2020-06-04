using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IEmpresaRepository : IBaseRepository<Empresa>
    {
        Empresa BuscarPorId(int id);
        Empresa BuscarPorCnpj(string cnpj);
        string ObterCodEmpresaRm(int idPrestador);
    }
}
