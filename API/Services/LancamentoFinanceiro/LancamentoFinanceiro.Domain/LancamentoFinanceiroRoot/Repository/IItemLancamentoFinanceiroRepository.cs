using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface IItemLancamentoFinanceiroRepository : IBaseRepository<ItemLancamentoFinanceiro>
    {
        List<ItemLancamentoFinanceiro> ObterItensLancamentoPorIdServicoContratadoPorPeriodo(List<int> idsServicos, DateTime? dtInicio, DateTime? dtFim);
        List<ItemLancamentoFinanceiro> ObterItensLancamentoPorIdRepasseRecebidoPorPeriodo(List<int> idsRepasses, DateTime? dtInicio, DateTime? dtFim);
        List<ItemLancamentoFinanceiro> ObterItensLancamentoPorIdRepassePagoPorPeriodo(List<int> idsRepasses, DateTime? dtInicio, DateTime? dtFim);
    }
}
