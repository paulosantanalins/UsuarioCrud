using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoPortifolio
{
    public class EscopoVM : ControleEdicaoVM
    {
        public int Id { get; set; }
        public string NmEscopoServico { get; set; }
        public bool FlAtivo { get; set; }

        public int IdPortfolioServico { get; set; }
        public virtual PortfolioServicoVM PortfolioServico { get; set; }

        public virtual ICollection<ServicoContratadoVM> ServicoContratados { get; set; }

    }
}
