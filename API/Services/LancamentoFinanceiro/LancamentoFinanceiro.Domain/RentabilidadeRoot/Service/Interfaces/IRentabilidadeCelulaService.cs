using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.RentabilidadeRoot.Service.Interfaces
{
    public interface IRentabilidadeCelulaService
    {
        List<ValoresRelatorioRentabilidadeDto> ObterInformacoesPorServicoProjeto(FiltroRelatorioRentabilidadeCelulaDto filtro);
    }
}
