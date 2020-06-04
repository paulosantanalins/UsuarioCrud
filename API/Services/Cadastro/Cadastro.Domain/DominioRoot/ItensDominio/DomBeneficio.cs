using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.ItensDominio
{
    public class DomBeneficio : Dominio
    {
        public IEnumerable<ValorPrestadorBeneficio> ValoresPrestadorBeneficioDom { get; set; }
    }
}
