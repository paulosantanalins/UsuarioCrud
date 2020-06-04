using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.Interfaces;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Repository
{
    public interface ITipoServicoRepository : IBaseRepository<TipoServico>
    {
        bool Validar(TipoServico model);
        FiltroGenericoDto<TipoServico> Filtrar(FiltroGenericoDto<TipoServico> filtro);
        bool ValidarInativação(int id);
    }
}
