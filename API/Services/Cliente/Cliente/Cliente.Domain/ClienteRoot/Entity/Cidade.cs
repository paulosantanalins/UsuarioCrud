using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Entity
{
    public class Cidade : EntityBase
    {
        public string NmCidade { get; set; }

        public int IdEstado { get; set; }
        public virtual Estado Estado { get; set; }

        public virtual ICollection<Endereco> Enderecos { get; set; }
    }
}
