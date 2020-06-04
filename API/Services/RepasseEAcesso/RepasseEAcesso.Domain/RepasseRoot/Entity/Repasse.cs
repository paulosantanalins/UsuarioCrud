using RepasseEAcesso.Domain.SharedRoot.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepasseEAcesso.Domain.RepasseRoot.Entity
{
    public class Repasse : EntityBase
    {
        public int IdCelulaOrigem { get; set; }
        public int? IdClienteOrigem { get; set; }
        public int? IdServicoOrigem { get; set; }

        public int IdCelulaDestino { get; set; }
        public int IdClienteDestino { get; set; }
        public int IdServicoDestino { get; set; }

        public DateTime DataRepasse { get; set; }

        public int? IdProfissional { get; set; }
        public decimal? ValorCustoProfissional { get; set; }
        public decimal? ValorUnitario { get; set; }
        public decimal? QuantidadeItens { get; set; }
        public decimal? ValorTotal { get; set; }
        public int IdMoeda { get; set; }
        public string DescricaoProjeto { get; set; }

        public string MotivoNegacao { get; set; }
        public string Justificativa { get; set; }
        public string Status { get; set; }

        public int? IdRepasseMae { get; set; }       
        
        public int? IdOrigem { get; set; }

        [NotMapped]
        public int IdEpm { get; set; }
        [NotMapped]
        public bool? RepasseInterno { get; set; }
        [NotMapped]
        public int? QtdVezesRepetir { get; set; }

        public Repasse Clone()
        {
            return this.MemberwiseClone() as Repasse;
        }
    }
}
