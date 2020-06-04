using RepasseEAcesso.Domain.CelulaRoot.Entity;
using RepasseEAcesso.Domain.DominioRoot.ItensDominio;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepasseEAcesso.Domain.RepasseRoot.Entity
{
    public class RepasseNivelUm : EntityBase
    {

        public RepasseNivelUm()
        {
            LogsRepasse = new HashSet<LogRepasse>();
        }
        public int IdCelulaOrigem { get; set; }
        public int IdClienteOrigem { get; set; }
        public string NomeClienteOrigem { get; set; }
        public int IdServicoOrigem { get; set; }
        public string  NomeServicoOrigem { get; set; }
        public int IdCelulaDestino { get; set; }
        public int IdClienteDestino { get; set; }
        public string NomeClienteDestino { get; set; }
        public int IdServicoDestino { get; set; }
        public string NomeServicoDestino { get; set; }
        public DateTime DataRepasse { get; set; }
        public int? IdProfissional { get; set; }
        public string NomeProfissional { get; set; }
        [NotMapped]
        public bool? ProfissionalAtivo { get; set; }
        public decimal? ValorCustoProfissional { get; set; }
        public decimal? ValorUnitario { get; set; }
        public int? QuantidadeItens { get; set; }
        public decimal? ValorTotal { get; set; }
        public int IdMoeda { get; set; }
        public string DescricaoProjeto { get; set; }

        public string MotivoNegacao { get; set; }
        public string Justificativa { get; set; }
        public string Status { get; set; }
        public DateTime DataLancamento { get; set; }
        public int? IdRepasseMae { get; set; } = null;
        public DateTime? DataRepasseMae { get; set; }
        public int? IdEpm { get; set; }
        public int? IdRepasseEacesso { get; set; }
        public int? IdRepasseMaeEAcesso { get; set; }
        public int? IdOrigem { get; set; }
        public bool RepasseInterno { get; set; }
        [NotMapped]
        public int? QtdVezesRepetir { get; set; }
        public string ParcelaRepasse { get; set; }
   

        public virtual Celula CelulaOrigem { get; set; }
        public virtual Celula CelulaDestino { get; set; }
        public virtual DomMoeda Moeda { get; set; }
        public virtual ICollection<LogRepasse> LogsRepasse { get; set; }
        public RepasseNivelUm Clone()
        {
            return this.MemberwiseClone() as RepasseNivelUm;
        }
    }
}
