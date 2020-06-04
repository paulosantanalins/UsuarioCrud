using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.RentabilidadeRoot.Service.Interfaces
{
    public interface IRentabilidadeDiretoriaService
    {
        List<ValoresRelatorioRentabilidadeDto> ObterInformacoesPorDiretoria(FiltroRelatorioRentabilidadeDiretoriaDto filtro);
    }
}
