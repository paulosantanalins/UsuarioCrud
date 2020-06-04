using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using System.Threading.Tasks;
using Utils.Base;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces
{
    public interface IDeParaServicoService
    {
        Task<FiltroGenericoDtoBase<GridServicoMigradoDTO>> Filtrar(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro);
        bool VerificarIdServicoEacessoExistente(int idServico);
        int BuscarIdServicoContratadoPorIdServicoEacesso(int idServicoEacesso);
        DeParaServico ObterServicoMigradoPorId(int id);
        void MigrarServicos();
        void MigrarMovimentacao();
    }
}
