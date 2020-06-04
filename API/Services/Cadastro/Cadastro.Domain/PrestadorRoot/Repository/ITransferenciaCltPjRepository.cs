using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Utils.Base;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface ITransferenciaCltPjRepository : IBaseRepository<TransferenciaCltPj>
    {
        FiltroGenericoDtoBase<TransferenciaCltPjDto> Filtrar(FiltroGenericoDtoBase<TransferenciaCltPjDto> filtro);
    }
}
