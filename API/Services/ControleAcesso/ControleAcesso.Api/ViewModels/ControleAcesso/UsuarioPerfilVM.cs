using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels.ControleAcesso
{
    public class UsuarioPerfilVM
    {
        public int IdPerfil { get; set; }
        public string LgUsuario { get; set; }
        public string NmPerfil { get; set; }
        public List<string> Funcionalidades { get; set; }
    }
}
