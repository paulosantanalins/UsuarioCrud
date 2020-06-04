using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using Utils.Base;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Repository
{
    public interface IDeParaServicoRepository : IBaseRepository<DeParaServico>
    {
        FiltroGenericoDtoBase<GridServicoMigradoDTO> Filtrar(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro);
        FiltroGenericoDtoBase<AgrupamentoDTO> FiltrarAgrupamentos(FiltroGenericoDtoBase<AgrupamentoDTO> filtro);
        int ObterIdServicoContratadoPorIdServicoEacesso(int idServicoEacesso);
        int ObterIdServicoContratadoPorIdServicoEacessoDapper(int idServicoEacesso);
        DeParaServico ObterDeParaPorId(int id);
    }
}
