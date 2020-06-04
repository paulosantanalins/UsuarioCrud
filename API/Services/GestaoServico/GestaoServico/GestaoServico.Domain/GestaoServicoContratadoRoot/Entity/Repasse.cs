using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Entity
{
    public class Repasse : EntityBase
    {
        public int? IdProfissional { get; set; }
        public DateTime DtRepasse { get; set; }
        public string DescProjeto { get; set; }
        public string DescMotivoNegacao { get; set; }
        public string DescJustificativa { get; set; }
        public int? QtdRepasse { get; set; }
        public decimal? VlUnitario { get; set; }
        public decimal? VlTotal { get; set; }
        public decimal? VlCustoProfissional { get; set; }
        public string FlStatus { get; set; }
        public DateTime? DtRepasseMae { get; set; }
        public int IdEpm { get; set; }
        public int IdMoeda { get; set; }
        public int IdServicoContratadoOrigem { get; set; }
        public int IdServicoContratadoDestino { get; set; }
        public int? IdRepasseMae { get; set; }
        //public int? IdTipoDespesa { get; set; }
        public int? NrParcela { get; set; }
        public bool FlRepasseInterno { get; set; }
        public int? IdPessoaAlteracao { get; set; }

        public virtual ServicoContratado ServicoContratadoOrigem { get; set; }
        public virtual ServicoContratado ServicoContratadoDestino { get; set; }
        public virtual Repasse RepasseMae { get; set; }
        public ICollection<Repasse> RepasseFilhos { get; set; }
        public int? IdRepasseEacesso { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [NotMapped]
        public int? IdTipoDespesa { get; set; }
        [NotMapped]
        public decimal? VlInc { get; set; }
        [NotMapped]
        public decimal? VlDesc { get; set; }

    }
}
