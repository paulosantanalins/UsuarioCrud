using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Entity
{
    public class Celula : EntityBase
    {
        public string DescCelula { get; set; }

        public bool FlHabilitarRepasseMesmaCelula { get; set; }
        public bool FlHabilitarRepasseEPM { get; set; }
        public int? IdCelulaPai { get; set; }
        public virtual Celula CelulaPai { get; set; }
        public virtual ICollection<Celula> CelulasSubordinadas { get; set; }

        public virtual ICollection<VinculoServicoCelulaComercial> VinculoServicoCelulaComercial { get; set; }

        //public virtual ICollection<ServicoContratado> ServicosContrados { get; set; }
    }
}
