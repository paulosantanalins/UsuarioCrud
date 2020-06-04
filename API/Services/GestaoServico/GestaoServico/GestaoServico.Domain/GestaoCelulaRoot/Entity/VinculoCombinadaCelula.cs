using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoCelulaRoot.Entity
{
    public class VinculoCombinadaCelula
    {
        public int Id { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }

        public int IdCelula { get; set; }
        public virtual Celula Celula { get; set; }
    }
}
