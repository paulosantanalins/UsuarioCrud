using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using System.Collections.Generic;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service.Interfaces
{
    public interface ILancamentoFinanceiroService
    {
        IEnumerable<RootLancamentoFinanceiro> ObterTodos();
        void AdicionarRange(List<RootLancamentoFinanceiro> rootLancamentos);
        void Adicionar(RootLancamentoFinanceiro rootLancamento);
        void RemoverLancamentosFinanceirosPorRepasse(int idRepasse);
    }
}
