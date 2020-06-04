using UsuarioApi.Domain.SharedRoot.UoW.Interfaces;
using System.Collections.Generic;
using UsuarioApi.Domain.DominioRoot.Entity;
using UsuarioApi.Domain.DominioRoot.Service.Interfaces;
using UsuarioApi.Domain.DominioRoot.Repository;

namespace UsuarioApi.Domain.DominioRoot.Service
{
    public class UsuarioService : IUsuarioService
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Usuario> BuscarTodos()
        {
            var todos = _usuarioRepository.BuscarTodos();
            return todos;
        }

        public void Persistir(Usuario usuario)
        {
            _usuarioRepository.Adicionar(usuario);
            _unitOfWork.Commit();
        }

        public void Editar(Usuario usuario)
        {
            _usuarioRepository.Atualizar(usuario);
            _unitOfWork.Commit();
        }

        public void Delete(Usuario usuario)
        {
            _usuarioRepository.Remove(usuario);
            _unitOfWork.Commit();
        }

        public Usuario BuscarPorId(int id)
        {
            var usuario = _usuarioRepository.BuscarPorId(id);

            return usuario;
        }

    }
}
