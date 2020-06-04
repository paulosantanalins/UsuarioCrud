using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto
{
    public class FiltroRelatorioRentabilidadeDiretoriaDto
    {
        public int IdMoeda { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public List<int> IdsCelula { get; set; }
        public string TipoRelatorio { get; set; }
    }
}
