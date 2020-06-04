using System.Collections.Generic;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
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

        //public virtual ICollection<VinculoCelulaContrato> VinculoCelulaContratos { get; set; }
        public virtual ICollection<ServicoContratado> ServicoContratados { get; set; }
    }
}
