using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using System.Collections.Generic;
using Utils;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface ILancamentoFinanceiroRepository : IBaseRepository<RootLancamentoFinanceiro>
    {
        List<RootLancamentoFinanceiro> ObterLancamentosPorRepasse(int idRepasse);
        List<RootLancamentoFinanceiro> BulkInsert(List<RootLancamentoFinanceiro> entities);
        void AdicionarRangeLancamentos(List<RootLancamentoFinanceiro> entities);
        int ObterIdServicoContratado(int idServicoEacesso);
    }
}
