using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomGraduacao : Dominio
    {
        public IEnumerable<Pessoa> Pessoas { get; set; }
    }
}