using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class ItemLancamentoFinanceiro : EntityBase
    {
        public int IdLancamentoFinanceiro { get; set; }
        public int? IdServicoContratado { get; set; }
        public decimal VlLancamento { get; set; }
        public int? IdRepasse { get; set; }
        public DateTime? DtRepasse { get; set; }
        public decimal? VlInc { get; set; }
        public decimal? VlDesc { get; set; }
        public virtual RootLancamentoFinanceiro LancamentoFinanceiro { get; set; }
        [NotMapped]
        public string CodigoCusto { get; set; }
    }
}
