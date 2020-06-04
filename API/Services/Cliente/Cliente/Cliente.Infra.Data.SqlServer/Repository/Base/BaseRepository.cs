using Cliente.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Cliente.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Cliente.Infra.Data.SqlServer.Context;
using System.Linq.Expressions;
using System.Linq;
using Cliente.Domain.SharedRoot;
using Utils;

namespace Cliente.Infra.Data.SqlServer.Repository.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly ClienteContext _context;
        private readonly IVariablesToken _variables;

        public BaseRepository(ClienteContext context, IVariablesToken variables)
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


        public ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public ICollection<TEntity> BuscarReadOnly(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate).ToList();
        }

        public TEntity BuscarPorId(int id)
        {
            return DbSet.Find(id);
        }

        public TEntity BuscarUnicoPorParametro(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public ICollection<TEntity> BuscarTodos()
        {
            return DbSet.ToList();
        }

        public ICollection<TEntity> BuscarTodosReadOnly()
        {
            return DbSet.AsNoTracking().ToList();
        }

        public ICollection<TEntity> BuscarTodosPaginado(Utils.FiltroGenericoDto<TEntity> filtro)
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

    }
}
