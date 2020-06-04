using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Repository;
using Utils.Base;

namespace RepasseEAcesso.Domain.PeriodoRepasseRoot.Repository
{
    public interface IPeriodoRepasseRepository : IBaseRepository<PeriodoRepasse>
    {
        FiltroGenericoDtoBase<PeriodoRepasseDto> FiltrarPeriodo(FiltroGenericoDtoBase<PeriodoRepasseDto> filtro);
        PeriodoRepasse BuscarPeriodoVigente();
    }
}
