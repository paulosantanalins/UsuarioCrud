using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ValorPrestadorBeneficio : EntityBase
    {
        public int IdValorPrestador { get; set; }
        public int IdBeneficio { get; set; }
        public decimal ValorBeneficio { get; set; }        
        public virtual DomBeneficio Beneficio { get; set; }
        public virtual ValorPrestador ValorPrestador { get; set; }
    }
}
