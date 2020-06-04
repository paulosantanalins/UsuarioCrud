using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class TipoDespesa : EntityBase
    {
        public string DescTipoDespesa { get; set; }
        public string SgTipoDespesa { get; set; }

        public virtual ICollection<RootLancamentoFinanceiro> LancamentosFinanceiros { get; set; }
    }
}
