using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.EnderecoRoot.Entity
{
    public class Cidade : EntityBase
    {
        public string NmCidade { get; set; }
        public int IdEstado { get; set; }

        public string CodIBGE { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual ICollection<Endereco> Enderecos { get; set; }
    }
}
