using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Entity
{
    public class TipoServico : EntityBase
    {
        public string DescTipoServico { get; set; }
        public bool FlStatus { get; set; }

        public virtual ICollection<PortfolioServico> PortifolioServicos { get; set; }
    }
}
