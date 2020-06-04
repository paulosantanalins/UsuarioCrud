using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using System.Collections.Generic;
using Utils.RM.Models;

namespace LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface
{
    public interface IMigracaoRMService
    {
        List<LancamentoFinanceiroRMDTO> BuscarLancamentosFinanceirosRM(string dtInicio, string dtFim);
        void MigrarLancamentosFinanceirosRM(IEnumerable<RootLancamentoFinanceiro> lancamentos);
    }
}
