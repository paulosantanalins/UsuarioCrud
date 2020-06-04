using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System.Collections.Generic;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces
{
    public interface IPortfolioServicoService
    {
        FiltroGenericoDto<PortfolioServicoDto> Filtrar(FiltroGenericoDto<PortfolioServicoDto> filtro);
        void Persistir(PortfolioServico portfolioServico);
        void Inativar(int id);
        PortfolioServico BuscarPorId(int id);
        IEnumerable<PortfolioServico> BuscarTodas();
        bool ValidarInexistencia(int id);
        IEnumerable<PortfolioServico> ObterTodos();
        IEnumerable<PortfolioServico> ObterTodosAtivos();
    }
}
