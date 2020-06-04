using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces
{
    public interface ICategoriaContabilService
    {
        FiltroGenericoDto<CategoriaContabil> Filtrar(FiltroGenericoDto<CategoriaContabil> filtro);
        void Persistir(CategoriaContabil classificacaoContabil);
        void Inativar(int id);
        CategoriaContabil BuscarPorId(int id);
        IEnumerable<CategoriaContabil> BuscarTodas();
        IEnumerable<CategoriaContabil> BuscarTodasAtivas();
        bool ValidarInativacaoPorId(int id);
    }
}
