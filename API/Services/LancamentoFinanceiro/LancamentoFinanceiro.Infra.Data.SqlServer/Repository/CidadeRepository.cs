using System;
using System.Linq;
using System.Linq.Expressions;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class CidadeRepository : BaseRepository<Cidade>, ICidadeRepository
    {
        public CidadeRepository(ServiceContext context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) :
            base(context, variables, auditoriaRepository)
        {
        }

        public Cidade BuscarUnicoPorParametro(Expression<Func<Cidade, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }
    }
}
