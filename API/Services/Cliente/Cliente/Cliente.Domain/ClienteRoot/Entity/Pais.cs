using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Entity
{
    public class Pais : EntityBase
    {
        public Pais()
        {
            Estados = new HashSet<Estado>();
        }
        public string SgPais { get; set; }
        public string NmPais { get; set; }

        public virtual ICollection<Estado> Estados { get; set; }
    }
}
