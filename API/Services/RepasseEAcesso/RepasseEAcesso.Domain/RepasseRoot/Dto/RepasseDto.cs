using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Domain.RepasseRoot.Dto
{
    public class RepasseDto
    {
        public int Id { get; set; }
        public int IdRepasse { get; set; }
        public int IdCelulaDestino { get; set; }
        public int IdCelulaOrigem { get; set; }
        public string ClienteDestino { get; set; }
        public string ServicoDestino { get; set; }
        public int? QuantidadeHoras { get; set; }
        public decimal? ValorUnitario { get; set; }
        public decimal? ValorRepasse { get; set; }
        public decimal? ValorTotal { get; set; }
        public decimal? ValorCustoProfissional { get; set; }
        public bool? Aprovar { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public bool? Desabilita { get; set; }
        public string Motivo { get; set; }
        public string Descricao { get; set; }
    }
}
