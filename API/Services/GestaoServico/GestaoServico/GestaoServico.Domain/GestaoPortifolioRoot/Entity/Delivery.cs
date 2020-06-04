
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Entity
{
    public class Delivery : EntityBase
    {
        public string DescDelivery { get; set; }
        public string SgDelivery { get; set; }
        public bool FlStatus { get; set; }
        public ICollection<PortfolioServico> PortifolioServicos { get; set; }
    }
}