using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class Perfil : EntityBase
    {
        public string NmPerfil { get; set; }
        public string NmModulo { get; set; }
        public bool FlAtivo { get; set; }

        public virtual ICollection<UsuarioPerfil> UsuarioPerfis { get; set; }
        public virtual ICollection<VinculoPerfilFuncionalidade> VinculoPerfilFuncionalidades { get; set; }
    }
}
