using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Entity
{
    public class PortfolioServico : EntityBase
    {
        public string NmServico { get; set; }
        public string DescServico { get; set; }
        public bool FlStatus { get; set; }
        public int? IdDelivery { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual ICollection<EscopoServico> EscopoServicos { get; set; }
    }
}
