using System;
using System.Collections.Generic;

namespace RepasseEAcesso.Domain.RepasseRoot.Dto
{
    public class RepasseNivelUmDto
    {
        public RepasseNivelUmDto()
        {
            LogsRepasse = new List<LogRepasseDto>();
        }

        public int Id { get; set; }
        public int IdCelulaOrigem { get; set; }
        public int IdClienteOrigem { get; set; }
        public string NomeClienteOrigem { get; set; }
        public int IdServicoOrigem { get; set; }
        public string NomeServicoOrigem { get; set; }
        public int IdCelulaDestino { get; set; }
        public int IdClienteDestino { get; set; }
        public string NomeClienteDestino { get; set; }
        public int IdServicoDestino { get; set; }
        public string NomeServicoDestino { get; set; }
        public int? IdProfissional { get; set; }
        public bool? ProfissionalAtivo { get; set; }
        public string NomeProfissional { get; set; }
        public decimal? ValorCustoProfissional { get; set; }
        public decimal ValorUnitario { get; set; }
        public int QuantidadeItens { get; set; }
        public decimal ValorTotal { get; set; }
        public int IdMoeda { get; set; }
        public string NomeMoeda { get; set; }
        public string DescricaoProjeto { get; set; }
        public string MotivoNegacao { get; set; }
        public string Justificativa { get; set; }
        public string Status { get; set; }
        public int? IdRepasseMae { get; set; }
        public string ParcelaRepasse { get; set; }


        public DateTime? DataAlteracao { get; set; }
        public DateTime DataRepasse { get; set; }
        public DateTime? DataRepasseMae { get; set; }
        public DateTime? DataLancamento { get; set; }

        public int IdEpm { get; set; }
        public int IdRepasseEacesso { get; set; }
        public int IdOrigem { get; set; }
        public bool RepasseInterno { get; set; }
        public string Usuario { get; set; }
        public int? QtdVezesRepetir { get; set; }
        public int IdPeriodoRepasse { get; set; }

        public List<LogRepasseDto> LogsRepasse { get; set; }
    }
}
