using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomContratacao : Dominio
    {
        public IEnumerable<Prestador> Prestadores { get; set; }
    }
}