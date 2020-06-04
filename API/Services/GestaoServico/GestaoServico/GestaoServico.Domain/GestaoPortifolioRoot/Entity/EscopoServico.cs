using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Entity
{
    public class EscopoServico : EntityBase
    {
        public EscopoServico()
        {
            //VinculoCelulaContratos = new HashSet<VinculoCelulaContrato>();
        }
        public string NmEscopoServico { get; set; }
        public bool FlAtivo { get; set; }
        public int IdPortfolioServico { get; set; }
        public virtual PortfolioServico PortfolioServico { get; set; }

        //public virtual ICollection<VinculoCelulaContrato> VinculoCelulaContratos { get; set; }
        public virtual ICollection<ServicoContratado> ServicoContratados { get; set; }
    }
}
