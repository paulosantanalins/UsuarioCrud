using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PessoaRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomNacionalidade : Dominio
    {
        public IEnumerable<Pessoa> Pessoas { get; set; }
    }
}