using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Entity
{
    public class Estado : EntityBase
    {
        public Estado()
        {
            Cidades = new HashSet<Cidade>();
        }
        public string SgEstado { get; set; }
        public string NmEstado { get; set; }

        public int IdPais { get; set; }
        public virtual Pais Pais { get; set; }

        public virtual ICollection<Cidade> Cidades { get; set; }
    }
}
