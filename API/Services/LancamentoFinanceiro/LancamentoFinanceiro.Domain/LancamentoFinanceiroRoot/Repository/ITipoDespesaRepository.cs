using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface ITipoDespesaRepository : IBaseRepository<TipoDespesa>
    {
        int ObterTipoDespesaPorSigla(string sigla);
    }
}
