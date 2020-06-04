using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using System.Collections.Generic;
using Utils.Base;

namespace GestaoServico.Domain.OperacaoMigradaRoot.Service.Interfaces
{
    public interface IOperacaoMigradaService
    {
        FiltroGenericoDtoBase<OperacaoMigradaDTO> Filtrar(FiltroGenericoDtoBase<OperacaoMigradaDTO> filtro);
        FiltroGenericoDtoBase<AgrupamentoDTO> FiltrarAgrupamentos(FiltroGenericoDtoBase<AgrupamentoDTO> filtro);
        List<OperacaoMigradaDTO> BuscarServicosPorGrupoDelivery(int idGrupoDelivery);
        void AtualizarStatus(List<int> idsServicos);
    }
}
