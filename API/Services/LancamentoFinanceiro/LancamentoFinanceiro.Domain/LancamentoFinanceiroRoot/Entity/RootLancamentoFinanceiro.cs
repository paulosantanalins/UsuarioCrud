using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class RootLancamentoFinanceiro : EntityBase
    {
        public RootLancamentoFinanceiro()
        {
            ItensLancamentoFinanceiro = new HashSet<ItemLancamentoFinanceiro>();
        }

        public string DescricaoOrigemLancamento { get; set; }
        public DateTime DtLancamento { get; set; }
        public DateTime DtBaixa { get; set; }
        public string DescricaoTipoLancamento { get; set; }
        public int? IdLan { get; set; }
        public string CodigoColigada { get; set; }
        public int? IdTipoDespesa { get; set; }
        public string DescOrigemCompraEacesso { get; set; }
        public decimal? VlImpDev { get; set; }
        public decimal? VlIr { get; set; }
        public decimal? VlIss { get; set; }
        public decimal? VlInssOp { get; set; }
        public decimal? VlDespesaRefeicao { get; set; }
        public decimal? VlDespesaTelefonia { get; set; }
        public decimal? VlDespesaTransporte { get; set; }
        public decimal? VlImpRet { get; set; }
        public decimal? VlDesconto { get; set; }
        public decimal? VlMulta { get; set; }
        public decimal? VlBaixado { get; set; }
        public decimal? VlOriginal { get; set; }
        public decimal? VlIrrf { get; set; }
        public decimal? VlJuros { get; set; }
        public decimal? VlAdiantamento { get; set; }
        public decimal? VlInss { get; set; }

        public virtual ICollection<ItemLancamentoFinanceiro> ItensLancamentoFinanceiro { get; set; }
        public virtual TipoDespesa TipoDespesa { get; set; }

        [NotMapped]
        public int? IdMov { get; set; }

    }
}
