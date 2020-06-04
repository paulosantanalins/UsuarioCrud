using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Utils;

namespace GestaoServico.Domain.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Adicionar(TEntity entity);
        void AdicionarRange(List<TEntity> entities);
        void AtualizarVariosSemAuditoria(List<TEntity> entities);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        ICollection<TEntity> BuscarTodosReadOnly();
        ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        ICollection<TEntity> BuscarTodosPaginado(FiltroGenericoDto<TEntity> filtro);

        void Update(TEntity t);
        void UpdateCompose(TEntity entity, object[] chaves);
        void Remover(TEntity entity);
    }
}
