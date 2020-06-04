using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System.Collections.Generic;
using GestaoServico.Domain.Dto;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces
{
    public interface IClassificacaoContabilService
    {
        FiltroGenericoDto<ClassificacaoContabilDto> Filtrar(FiltroGenericoDto<ClassificacaoContabilDto> filtro);
        void Persistir(ClassificacaoContabil classificacaoContabil);
        void Inativar(int id);
        ClassificacaoContabil BuscarPorId(int id);
        ICollection<ClassificacaoContabil> obterClassificacoesPorCategoria(int idCategoria);
        bool ValidarInativacaoPorId(int id);
    }
}
