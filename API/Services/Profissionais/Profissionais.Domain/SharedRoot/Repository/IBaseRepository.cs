using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UsuarioApi.Domain.SharedRoot.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Adicionar(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Atualizar(TEntity t);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        TEntity BuscarPrimeiro(Expression<Func<TEntity, bool>> predicate);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void DetachAllEntities(IEnumerable<TEntity> entities);
    }
}
