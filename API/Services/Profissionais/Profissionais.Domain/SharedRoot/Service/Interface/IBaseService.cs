﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UsuarioApi.Domain.SharedRoot.Service.Interface
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        void Adicionar(TEntity entity);
        void Atualizar(TEntity entity);
        TEntity BuscarPorId(int id);
        ICollection<TEntity> BuscarTodos();
        IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate);
    }
}
