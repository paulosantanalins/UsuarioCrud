using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Domain.RepasseRoot.Dto
{
    public class AprovarRepasseNivelDoisDto
    {
        public int Id { get; set; }
        public int IdRepasse { get; set; }
        public int IdCelulaDestino { get; set; }
        public int IdCelulaOrigem { get; set; }
        public string ClienteOrigem { get; set; }
        public string ClienteDestino { get; set; }
        public string ServicoOrigem { get; set; }
        public string ServicoDestino { get; set; }
        public int? QuantidadeHoras { get; set; }
        public decimal? ValorUnitario { get; set; }        
        public decimal? ValorTotal { get; set; }
        public decimal? ValorCustoProfissional { get; set; }
        public bool? Aprovado { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public bool? EstaNoPeriodoVigente { get; set; }
        public string Motivo { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataRepasse { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        List<LogRepasseDto> LogsRepasses = new List<LogRepasseDto>();
    }
}
