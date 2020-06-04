using RepasseEAcesso.Domain.DominioRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Repository;

namespace RepasseEAcesso.Domain.DominioRoot.Repository
{
    public interface IDominioRepository : IBaseRepository<Dominio>
    {
        Dominio Buscar(int idValor, string tipoDominio);
    }
}
