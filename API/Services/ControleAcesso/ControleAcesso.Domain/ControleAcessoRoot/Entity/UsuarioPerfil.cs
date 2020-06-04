using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class UsuarioPerfil : EntityBase
    {
        public int IdPerfil { get; set; }
        public string LgUsuario { get; set; }
        public virtual Perfil Perfil { get; set; }
    }
}
