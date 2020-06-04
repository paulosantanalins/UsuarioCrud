using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Utils;

namespace ControleAcesso.Domain.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Adicionar(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        TEntity AdicionarComRetorno(TEntity entity);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        TEntity BuscarFirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        ICollection<TEntity> BuscarTodosPaginado(FiltroGenericoDto<TEntity> filtro);
        void Update(TEntity t);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
