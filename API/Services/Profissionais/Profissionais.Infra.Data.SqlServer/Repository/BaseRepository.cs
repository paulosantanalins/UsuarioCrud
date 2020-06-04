using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UsuarioApi.Domain.SharedRoot.Entity;
using UsuarioApi.Domain.SharedRoot.Repository;
using UsuarioApi.Infra.Data.SqlServer.Context;

namespace UsuarioApi.Infra.Data.SqlServer.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            DbSet = _context.Set<TEntity>();
        }

        public void Adicionar(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(x =>
            {
                return x;
            });

            DbSet.AddRange(entities);
        }

        public void Atualizar(TEntity entity)
        {
            var entityDB = DbSet.Find(entity.Id);

            _context.Entry(entityDB).CurrentValues.SetValues(entity);

            DbSet.Update(entityDB);
        }

        public TEntity BuscarPrimeiro(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }


        public IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate);
        }

        public TEntity BuscarPorId(int id)

        {
            return DbSet.Find(id);
        }

        public ICollection<TEntity> BuscarTodos()
        {
            return DbSet.ToList();
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public void DetachAllEntities(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
        }
    }
}
