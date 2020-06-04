using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class Funcionalidade : EntityBase
    {
        public string NmFuncionalidade { get; set; }
        public string DescFuncionalidade { get; set; }
        public bool FlAtivo { get; set; }
        public virtual ICollection<VinculoPerfilFuncionalidade> VinculoPerfilFuncionalidades { get; set; }
    }
}
