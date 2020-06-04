using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.EnderecoRoot.Entity
{
    public class Estado : EntityBase
    {
        public string SgEstado { get; set; }
        public string NmEstado { get; set; }
        public int IdPais { get; set; }

        public virtual Pais Pais { get; set; }

        public virtual ICollection<Cidade> Cidades { get; set; }
    }
}
