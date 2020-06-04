using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class TipoDespesaRepository : BaseRepository<TipoDespesa>, ITipoDespesaRepository
    {
        public TipoDespesaRepository(ServiceContext servicoContext, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(servicoContext, variables, auditoriaRepository)
        {

        }

        public int ObterTipoDespesaPorSigla(string sigla)
        {
            if (sigla != null)
            {
                var result = DbSet.FirstOrDefault(x => x.SgTipoDespesa.ToUpper() == sigla.ToUpper());
                return result != null ? result.Id : 0;
            }

            return 0;
        }
    }
}
