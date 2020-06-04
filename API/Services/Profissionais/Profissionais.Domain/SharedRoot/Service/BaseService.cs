using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UsuarioApi.Domain.SharedRoot.Entity;
using UsuarioApi.Domain.SharedRoot.Repository;
using UsuarioApi.Domain.SharedRoot.Service.Interface;
using UsuarioApi.Domain.SharedRoot.UoW.Interfaces;

namespace UsuarioApi.Domain.SharedRoot.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : EntityBase
    {
        private readonly IBaseRepository<TEntity> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(
            IBaseRepository<TEntity> baseRepository,
            IUnitOfWork unitOfWork)
        {
            _baseRepository = baseRepository;
            _unitOfWork = unitOfWork;
        }

        public void Adicionar(TEntity entity)
        {
            _baseRepository.Adicionar(entity);
            _unitOfWork.Commit();
        }

        public void Atualizar(TEntity entity)
        {
            _baseRepository.Atualizar(entity);
            _unitOfWork.Commit();
        }

        public IEnumerable<TEntity> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            var result = _baseRepository.Buscar(predicate);
            return result;
        }

        public TEntity BuscarPorId(int id)
        {
            var result = _baseRepository.BuscarPorId(id);
            return result;
        }

        public ICollection<TEntity> BuscarTodos()
        {
            var result = _baseRepository.BuscarTodos();
            return result;
        }

    }
}
