using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Dapper;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class LancamentoFinanceiroRepository : BaseRepository<RootLancamentoFinanceiro>, ILancamentoFinanceiroRepository
    {
        protected IVariablesToken _variables;
        public LancamentoFinanceiroRepository(ServiceContext servicoContext, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(servicoContext, variables, auditoriaRepository)
        {
            _variables = variables;
        }


        public List<RootLancamentoFinanceiro> ObterLancamentosPorRepasse(int idRepasse)
        {
            var lancamentos = DbSet.Include(x => x.ItensLancamentoFinanceiro)
                                    .Where(x => x.ItensLancamentoFinanceiro.Any(y => y.IdRepasse == idRepasse)).ToList();
            return lancamentos;
        }

        public List<RootLancamentoFinanceiro> BulkInsert(List<RootLancamentoFinanceiro> entities)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ServiceContext context = null;
                try
                {
                    _variables.UserName = "EAcesso";
                    context = new ServiceContext(_variables, _auditoriaRepository);
                    context.ChangeTracker.AutoDetectChangesEnabled = false;

                    int count = 0;
                    foreach (var entityToInsert in entities)
                    {
                        entityToInsert.LgUsuario = "EAcesso";
                        ++count;
                        context = AddToContext(context, entityToInsert, count, 100, true);
                    }

                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                scope.Complete();
            }

            return entities;

        }

        private ServiceContext AddToContext(ServiceContext context,
                RootLancamentoFinanceiro entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<RootLancamentoFinanceiro>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new ServiceContext(_variables, _auditoriaRepository);
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        public void AdicionarRangeLancamentos(List<RootLancamentoFinanceiro> entities)
        {
            ServiceContext context = null;
            _variables.UserName = "Eacesso";
            try
            {
                context = new ServiceContext(_variables, _auditoriaRepository);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                int count = 0;
                foreach (var entityToInsert in entities)
                {
                    ++count;
                    context = AddToContext(context, entityToInsert, count, 50, true);
                }

                context.SaveChanges();
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        public int ObterIdServicoContratado(int idServicoEacesso)
        {
            var query = $@"SELECT IDSERVICOCONTRATADO
                              FROM TBLDEPARASERVICO
                              WHERE IDSERVICOEACESSO = {idServicoEacesso}";

            var result = _context.Database.GetDbConnection().QueryFirstOrDefault<int>(query);
            _context.Database.GetDbConnection().Close();

            return result;
        }
    }
}
