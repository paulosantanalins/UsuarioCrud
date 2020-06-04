using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces
{
    public interface ITipoServicoService
    {
        FiltroGenericoDto<TipoServico> Filtrar(FiltroGenericoDto<TipoServico> filtro);
        void Persistir(TipoServico tipoServico);
        void Inativar(int id);
        TipoServico BuscarPorId(int id);
        IEnumerable<TipoServico> BuscarTodos();
        bool VerificarExisteciaPorId(int id);
        IEnumerable<TipoServico> BuscarTodosAtivos();
    }
}
