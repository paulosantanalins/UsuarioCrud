using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Entity
{
    public class TipoDespesa : EntityBase
    {
        public string DescTipoDespesa { get; set; }
        public string SgTipoDespesa { get; set; }

        public virtual ICollection<Repasse> Repasses { get; set; }
    }
}
