using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.Interfaces;

namespace ControleAcesso.Domain.DominioRoot.Repository
{
    public interface IDominioRepository : IBaseRepository<Dominio>
    {
        int? ObterIdPeloCodValor(int idValor, string tipoDominio);
        int ObterIdDominio(string tipoDominio, string valorDominio);
    }
}
