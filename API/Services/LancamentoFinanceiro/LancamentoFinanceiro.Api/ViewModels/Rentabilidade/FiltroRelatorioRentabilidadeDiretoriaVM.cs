using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LancamentoFinanceiro.Api.ViewModels.Rentabilidade
{
    public class FiltroRelatorioRentabilidadeDiretoriaVM
    {
        public int IdMoeda { get; set; }
        public string DtInicio { get; set; }
        public string DtFim { get; set; }
        public List<int> IdsCelula { get; set; }
        public string TipoRelatorio { get; set; }
    }
}
