using LancamentoFinanceiro.Domain;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly ServiceContext _context;
        private readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public BaseRepository(ServiceContext context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository)
        {
            _context = context;
            _variables = variables;
            DbSet = _context.Set<TEntity>();
            _auditoriaRepository = auditoriaRepository;
        }

        public void Adicionar(TEntity entity)
        {
            entity.LgUsuario = _variables.UserName;
            entity.DtAlteracao = DateTime.Now;
            DbSet.Add(entity);
        }

        public ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public TEntity BuscarPorId(int id)
        {
            return DbSet.Find(id);
        }

        public ICollection<TEntity> BuscarTodos()
        {
            return DbSet.ToList();
        }

        public ICollection<TEntity> BuscarTodosPaginado(FiltroGenericoDto<TEntity> filtro)
        {
            return DbSet.Skip(filtro.Pagina).Take(filtro.QuantidadePorPagina).ToList();
        }

        public void Update(TEntity entity)
        {
            var entityDB = DbSet.Find(entity.Id);
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            entity.LgUsuario = _variables.UserName;
            entity.DtAlteracao = DateTime.Now;
            DbSet.Update(entityDB);
        }

        public void UpdateCompose(TEntity entity, object[] chaves)
        {
            var entityDB = DbSet.Find(chaves);
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            DbSet.Update(entityDB);
        }

        public void AdicionarRange(List<TEntity> entities)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            _variables.UserName = "Eacesso";
            foreach (var entity in entities)
            {
                entity.LgUsuario = _variables.UserName;
                entity.DtAlteracao = DateTime.Now;
                DbSet.Add(entity);
            }
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        


    }
}
