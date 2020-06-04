using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.Interfaces;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Repository
{
    public interface IPortfolioServicoRepository : IBaseRepository<PortfolioServico>
    {
        bool Validar(PortfolioServico portfolioServico);
        FiltroGenericoDto<PortfolioServicoDto> Filtrar(FiltroGenericoDto<PortfolioServicoDto> filtro);
        bool ValidarInativacao(int id);
    }
}
