using Cadastro.Domain.PrestadorRoot.Dto;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface ITransferenciaCltPjService
    {
        FiltroGenericoDtoBase<TransferenciaCltPjDto> Filtrar(FiltroGenericoDtoBase<TransferenciaCltPjDto> filtro);
        string Adicionar(DadosCltEacessoDto dadosCltEacesso);
    }
}
