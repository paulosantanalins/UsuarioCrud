using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Seguranca.Domain.UsuarioRoot.Repository
{
    public interface IUsuarioRepository
    {
        Task InserirUsuario(Usuario usuario);
        Task<List<Usuario>> GetAll();
        Task<bool> ValidarNomeUsuario(string nomeUsuario);
    }
}
