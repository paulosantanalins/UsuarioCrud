using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class DescontoPrestador : EntityBase
    {
        public int IdDesconto { get; set; }
        public int IdHorasMesPrestador { get; set; }
        public decimal ValorDesconto { get; set; }
        public string DescricaoDesconto { get; set; }
        public virtual HorasMesPrestador HorasMesPrestador { get; set; }
        public virtual DomDesconto Desconto { get; set; }
    }
}
