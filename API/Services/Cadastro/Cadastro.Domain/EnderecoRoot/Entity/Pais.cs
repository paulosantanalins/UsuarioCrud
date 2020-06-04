using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.EnderecoRoot.Entity
{
    public class Pais : EntityBase
    {
        public string SgPais { get; set; }
        public string NmPais { get; set; }

        public virtual ICollection<Estado> Estados { get; set; }
    }
}
