using System;
using System.Linq.Expressions;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface ICidadeRepository : IBaseRepository<Cidade>
    {
        Cidade BuscarUnicoPorParametro(Expression<Func<Cidade, bool>> predicate);
    }
}
