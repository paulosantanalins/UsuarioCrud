using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class VinculoPerfilFuncionalidade : EntityBase
    {
        public int IdFuncionalidade { get; set; }
        public int IdPerfil { get; set; }

        public virtual Perfil Perfil { get; set; }
        public virtual Funcionalidade Funcionalidade { get; set; }
    }
}
