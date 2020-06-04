using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Forecast.Domain.SharedRoot
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Adicionar(TEntity entity);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity t);
        void UpdateList(ICollection<TEntity> entities, bool deleteRemoved);
        void UpdateCompose(TEntity entity, object[] chaves);
        void AdicionarRange(List<TEntity> entities);
        void Remove(TEntity entity);
    }
}
