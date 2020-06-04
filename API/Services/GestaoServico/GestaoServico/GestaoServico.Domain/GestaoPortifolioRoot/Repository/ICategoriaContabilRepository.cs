using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Repository
{
    public interface ICategoriaContabilRepository : IBaseRepository<CategoriaContabil>
    {
        bool Validar(CategoriaContabil categoria);
        FiltroGenericoDto<CategoriaContabil> Filtrar(FiltroGenericoDto<CategoriaContabil> filtro);
        List<CategoriaContabil> ObterAtivos();
        bool ValidarInexistencia(int id);
    }
}
