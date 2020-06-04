using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoRoot.Entity
{
    public class TipoServico
    {
        public int Id { get; set; }
        public string SgTipoServico { get; set; }
        public string DescTipoServico { get; set; }

        public virtual ICollection<VinculoServicoTipoServico> VinculoServicoTipoServicos { get; set; }
    }
}
