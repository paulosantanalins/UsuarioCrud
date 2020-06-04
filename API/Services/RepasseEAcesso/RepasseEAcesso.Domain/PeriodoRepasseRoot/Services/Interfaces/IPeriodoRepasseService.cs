using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using System;
using Utils.Base;

namespace RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces
{
    public interface IPeriodoRepasseService
    {
        PeriodoRepasse BuscarPorId(int id);
        PeriodoRepasse BuscarPeriodoVigente();
        FiltroGenericoDtoBase<PeriodoRepasseDto> FiltrarPeriodo(FiltroGenericoDtoBase<PeriodoRepasseDto> filtro);
        void Persistir(PeriodoRepasse periodoRepasse);
        DateTime ObterDataDiasUteis(DateTime data, int qtdDias);
        void PopularDataFimLancamentoEacesso(PeriodoRepasse periodoVigente);
        void PopularDataFimEacesso(PeriodoRepasse periodoVigente);
    }
}
