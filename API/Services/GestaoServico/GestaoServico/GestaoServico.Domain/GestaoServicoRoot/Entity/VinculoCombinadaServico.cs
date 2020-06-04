using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoRoot.Entity
{
    public class VinculoCombinadaServico
    {
        public int Id { get; set; }
        public DateTime DtMigracao { get; set; }
        public string FlStatus { get; set; }

        public int IdServico { get; set; }
        public virtual Servico Servico { get; set; }

    }
}
