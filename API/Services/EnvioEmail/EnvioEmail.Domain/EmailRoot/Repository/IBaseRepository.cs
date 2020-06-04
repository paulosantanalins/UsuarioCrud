using EnvioEmail.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EnvioEmail.Domain.EmailRoot.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    {
        void Adicionar(TEntity entity);
        TEntity BuscarPorId(int id);
        TEntity BuscarPorIdReadOnly(int id);
        ICollection<TEntity> BuscarTodos();
        ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity t);
    }
}
