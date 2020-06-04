using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomDesconto : Dominio
    {
        public IEnumerable<DescontoPrestador> DescontosPrestador { get; set; }
    }
}
