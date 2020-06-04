using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Utils;

namespace Cliente.Domain.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    {
        void Adicionar(TEntity entity);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        ICollection<TEntity> BuscarTodosReadOnly();
        ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        ICollection<TEntity> BuscarReadOnly(Expression<Func<TEntity, bool>> predicate);
        TEntity BuscarUnicoPorParametro(Expression<Func<TEntity, bool>> predicate);
        ICollection<TEntity> BuscarTodosPaginado(FiltroGenericoDto<TEntity> filtro);
        void Update(TEntity t);
    }
}
