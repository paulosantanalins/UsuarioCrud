using GestaoServico.Domain;
using GestaoServico.Domain.Interfaces;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly GestaoServicoContext _context;
        private readonly IVariablesToken _variables;

        public BaseRepository(GestaoServicoContext context, IVariablesToken variables)
        {
            _context = context;
            _variables = variables;
            DbSet = _context.Set<TEntity>();
        }

        public void Adicionar(TEntity entity)
        {
            entity.Usuario = _variables.UserName;
            entity.DataAlteracao = DateTime.Now;
            DbSet.Add(entity);
        }

        public void AdicionarRange(List<TEntity> entities)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            if (string.IsNullOrEmpty(_variables.UserName))
            {
                _variables.UserName = "Eacesso";
            }

            foreach (var entity in entities)
            {
                entity.Usuario = _variables.UserName;
                if (entity.DataAlteracao == null)
                {
                    entity.DataAlteracao = DateTime.Now;
                }
                DbSet.Add(entity);
            }
        }

        public void AtualizarVariosSemAuditoria(List<TEntity> entities)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            DbSet.UpdateRange(entities);
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

        public ICollection<TEntity> BuscarTodosReadOnly()
        {
            return DbSet.AsNoTracking().ToList();
        }

        public ICollection<TEntity> BuscarTodosPaginado(FiltroGenericoDto<TEntity> filtro)
        {
            return DbSet.Skip(filtro.Pagina).Take(filtro.QuantidadePorPagina).ToList();
        }

        public void Update(TEntity entity)
        {
            var entityDB = DbSet.Find(entity.Id);
            entity.Usuario = _variables.UserName;
            entity.DataAlteracao = DateTime.Now;
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            DbSet.Update(entityDB);
        }

        public void UpdateCompose(TEntity entity, object[] chaves)
        {
            var entityDB = DbSet.Find(chaves);
            entity.Usuario = _variables.UserName;
            entity.DataAlteracao = DateTime.Now;
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            DbSet.Update(entityDB);
        }

        public void Remover(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}
