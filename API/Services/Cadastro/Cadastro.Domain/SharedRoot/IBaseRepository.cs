using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cadastro.Domain.SharedRoot
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Adicionar(TEntity entity);
        void Adicionar(TEntity entity, string usuarioAlteracao);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        TEntity BuscarPrimeiro(Expression<Func<TEntity, bool>> predicate);
        ICollection<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity t);
        void Update(TEntity entity, string usuarioAlteracao);
        void UpdateList(ICollection<TEntity> entities, bool deleteRemoved);
        void UpdateCompose(TEntity entity, object[] chaves);
        void UpdateRange(IEnumerable<TEntity> entities);
        void AdicionarRange(List<TEntity> entities);
        void Remove(TEntity entity);
        IEnumerable<EntityEntry> GetTrackedEntityBaseEntities();
    }
}
