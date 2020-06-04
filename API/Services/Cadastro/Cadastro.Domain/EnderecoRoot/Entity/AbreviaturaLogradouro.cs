using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.EnderecoRoot.Entity
{
    public class AbreviaturaLogradouro: EntityBase
    {
        public string Sigla { get; set; }

        public string Descricao { get; set; }

        public virtual ICollection<Endereco> Enderecos { get; set; }

    }
}





