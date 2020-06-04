using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Repository;
using EnvioEmail.Domain.SharedRoot.Entity;
using EnvioEmail.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;

namespace EnvioEmail.Infra.Data.SqlServer.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly ServiceBContext _context;

        public BaseRepository(ServiceBContext context)
        {
            _context = context;
            DbSet = _context.Set<TEntity>();
        }

        public void Adicionar(TEntity entity)
        {
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

        public TEntity BuscarPorIdReadOnly(int id)
        {
            return DbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public ICollection<TEntity> BuscarTodos()
        {
            return DbSet.ToList ();
        }

        public void Update(TEntity entity)
        {
            var entityDB = DbSet.Find(entity.Id);
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            DbSet.Update(entityDB);
        }
        
    }
}
