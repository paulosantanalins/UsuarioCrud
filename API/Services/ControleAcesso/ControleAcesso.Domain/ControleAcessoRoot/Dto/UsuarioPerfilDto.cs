using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class UsuarioPerfilDto
    {
        public UsuarioPerfilDto()
        {
            //Perfis = new List<string>();
        }
        public int Id { get; set; }
        public int IdPerfil { get; set; }
        public string LgUsuario { get; set; }
        public string NmPerfil { get; set; }
        public List<string> Funcionalidades { get; set; }
        public string Perfis { get; set; }
        public string Usuario { get; set; }
        public string NmUsuario { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public List<int> IdsPerfis { get; set; }
    }
}
