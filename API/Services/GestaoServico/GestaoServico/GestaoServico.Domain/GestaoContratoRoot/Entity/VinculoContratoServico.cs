using GestaoServico.Domain.GestaoServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoContratoRoot.Entity
{
    public class VinculoContratoServico
    {
        public int IdContrato { get; set; }
        public int IdServico { get; set; }

        public virtual Contrato Contrato { get; set; }
        public virtual Servico Servico { get; set; }
    }
}
