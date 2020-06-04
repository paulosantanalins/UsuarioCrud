using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomAreaFormacao : Dominio
    {
        public IEnumerable<Prestador> Prestadores { get; set; }
    }
}
