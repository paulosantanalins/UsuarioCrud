using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoRoot.Entity
{
    public class VinculoServicoTipoServico
    {
        public int IdServico { get; set; }
        public int IdTipoServico { get; set; }

        public virtual Servico Servico { get; set; }
        public virtual TipoServico TipoServico { get; set; }
    }
}
