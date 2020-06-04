using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly CadastroContexto _context;
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public BaseRepository(CadastroContexto context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository)
        {
            _context = context;
            _variables = variables;
            DbSet = _context.Set<TEntity>();
            _auditoriaRepository = auditoriaRepository;
        }

        public void Adicionar(TEntity entity)
        {
            entity.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            if (entity.DataAlteracao == null)
            {
                entity.DataAlteracao = DateTime.Now;
            }
            DbSet.Add(entity);
        }

        public void Adicionar(TEntity entity, string usuarioAlteracao)
        {
            entity.Usuario = usuarioAlteracao;
            if (entity.DataAlteracao == null)
            {
                entity.DataAlteracao = DateTime.Now;
            }
            DbSet.Add(entity);
        }

        public ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }
        public TEntity BuscarPrimeiro(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
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

            entityDB.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            entityDB.DataAlteracao = DateTime.Now;
            DbSet.Update(entityDB);
        }

        public void Update(TEntity entity, string usuarioAlteracao)
        {
            var entityDB = DbSet.Find(entity.Id);
            _context.Entry(entityDB).CurrentValues.SetValues(entity);

            entityDB.Usuario = usuarioAlteracao;
            entityDB.DataAlteracao = DateTime.Now;
            DbSet.Update(entityDB);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _context.UpdateRange(entities);
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

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }


        public IEnumerable<EntityEntry> GetTrackedEntityBaseEntities()
        {
            IEnumerable<EntityEntry> entities = _context.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);
            return entities;
        }

        public void RemoveRange(List<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }


        public void UpdateList(ICollection<TEntity> entities, bool deleteRemoved)
        {
            foreach (var entity in entities)
            {
                if (entity.Id == 0)
                {
                    Adicionar(entity);
                }
                else
                {
                    Update(entity);
                }
            }
            if (deleteRemoved)
            {
                var actualValues = DbSet.AsNoTracking().ToList();
                var removeds = actualValues.Where(x => !entities.Any(y => y.Id == x.Id)).ToList();
                RemoveRange(removeds);
            }
        }

    }
}
