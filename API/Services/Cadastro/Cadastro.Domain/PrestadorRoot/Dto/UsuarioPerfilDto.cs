using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class UsuarioPerfilDto
    {
        public string LgUsuario { get; set; }
        public List<string> Funcionalidades { get; set; }
        public string Perfis { get; set; }
    }
}
