using ControleAcesso.Domain;
using ControleAcesso.Domain.Interfaces;
using ControleAcesso.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected DbSet<TEntity> DbSet;
        public readonly ControleAcessoContext _context;
        private readonly IVariablesToken _variables;

        public BaseRepository(ControleAcessoContext context, IVariablesToken variables)
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

        public void AddRange(IEnumerable<TEntity> entities)
        {
            entities = entities.Select(x => { x.DataAlteracao = DateTime.Now; x.Usuario = _variables.UserName; return x; });
            DbSet.AddRange(entities);
        }


        public TEntity AdicionarComRetorno(TEntity entity)
        {
            entity.Usuario = _variables.UserName;
            entity.DataAlteracao = DateTime.Now;
            DbSet.Add(entity);
            return entity;
        }

        public ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public TEntity BuscarFirstOrDefault(Expression<Func<TEntity, bool>> predicate) => DbSet.FirstOrDefault(predicate);

        public TEntity BuscarPorId(int id)
        {
            return DbSet.Find(id);
        }

        public ICollection<TEntity> BuscarTodos() => DbSet.ToList();


        public ICollection<TEntity> BuscarTodosPaginado(FiltroGenericoDto<TEntity> filtro)
        {
            return DbSet.Skip(filtro.Pagina).Take(filtro.QuantidadePorPagina).ToList();
        }

        public void Update(TEntity entity)
        {
            var entityDB = DbSet.Find(entity.Id);
            _context.Entry(entityDB).CurrentValues.SetValues(entity);
            entityDB.Usuario = _variables.UserName;
            entityDB.DataAlteracao = DateTime.Now;
            DbSet.Update(entityDB);
        }


        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

    }
}
