using GestaoServico.Domain.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using GestaoServico.Domain.OperacaoMigradaRoot.Entity;
using System.Collections.Generic;
using Utils.Base;

namespace GestaoServico.Domain.OperacaoMigradaRoot.Repository
{
    public interface IOperacaoMigradaRepository : IBaseRepository<OperacaoMigrada>
    {
        FiltroGenericoDtoBase<OperacaoMigradaDTO> Filtrar(FiltroGenericoDtoBase<OperacaoMigradaDTO> filtro);
        List<OperacaoMigradaDTO> BuscarServicosPorGrupoDelivery(int idGrupoDelivery);
    }
}
