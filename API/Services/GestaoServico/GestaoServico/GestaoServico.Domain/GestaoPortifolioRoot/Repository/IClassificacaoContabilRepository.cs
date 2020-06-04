using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using GestaoServico.Domain.Dto;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Repository
{
    public interface IClassificacaoContabilRepository : IBaseRepository<ClassificacaoContabil>
    {
        bool Validar(ClassificacaoContabil model);
        FiltroGenericoDto<ClassificacaoContabilDto> Filtrar(FiltroGenericoDto<ClassificacaoContabilDto> filtro);
        ICollection<ClassificacaoContabil> BuscarClassificacoes(int id);
        bool ValidarDescricao(ClassificacaoContabil model);
        bool ValidarInexistencia(int id);
    }
}
