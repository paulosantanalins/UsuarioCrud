using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto
{
    public class FiltroRelatorioRentabilidadeCelulaDto
    {
        public int Situacao { get; set; }
        public int IdCelula { get; set; }
        public int IdMoeda { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public int? IdCliente { get; set; }
        public int? IdServicoContratado { get; set; }
    }
}
