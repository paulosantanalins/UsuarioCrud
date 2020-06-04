using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LancamentoFinanceiro.Api.ViewModels.Rentabilidade
{
    public class FiltroRelatorioRentabilidadeCelulaVM
    {
        public int Situacao { get; set; }
        public int IdCelula { get; set; }
        public int IdMoeda { get; set; }
        public string DtInicio { get; set; }
        public string DtFim { get; set; }
        public int? IdCliente { get; set; }
        public int? IdServicoContratado { get; set; }
    }
}
