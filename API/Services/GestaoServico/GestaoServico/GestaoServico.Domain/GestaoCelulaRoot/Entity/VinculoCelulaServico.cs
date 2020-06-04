using System;
using System.Collections.Generic;
using System.Text;
using GestaoServico.Domain.GestaoServicoRoot.Entity;

namespace GestaoServico.Domain.GestaoCelulaRoot.Entity
{
    public class VinculoCelulaServico
    {
        public int IdCelula { get; set; }
        public int IdServico { get; set; }

        public virtual Celula Celula { get; set; }
        public virtual Servico Servico { get; set; }
    }
}
