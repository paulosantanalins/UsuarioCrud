using System.Collections.Generic;
using UsuarioApi.Domain.DominioRoot.Entity;

namespace UsuarioApi.Domain.DominioRoot.Service.Interfaces
{
    public interface IUsuarioService
    {
        IEnumerable<Usuario> BuscarTodos();
        void Persistir(Usuario usuario);
        void Editar(Usuario usuario);
        Usuario BuscarPorId(int id);
        void Delete(Usuario usuario);



    }
}
