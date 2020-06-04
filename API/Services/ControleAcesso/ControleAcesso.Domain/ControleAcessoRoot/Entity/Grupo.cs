using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Entity
{
    public class Grupo : EntityBase
    {
        public string DescGrupo { get; set; }
        public bool Ativo { get; set; }

        public virtual ICollection<Celula> Celulas { get; set; }
    }
}
