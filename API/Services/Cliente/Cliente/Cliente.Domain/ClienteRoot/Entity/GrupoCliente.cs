using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Entity
{
    public class GrupoCliente : EntityBase
    {
        public string DescGrupoCliente { get; set; }
        public bool FlStatus { get; set; }
        public string IdClienteMae { get; set; }
        public virtual ICollection<ClienteET> Clientes { get; set; }
    }
}
