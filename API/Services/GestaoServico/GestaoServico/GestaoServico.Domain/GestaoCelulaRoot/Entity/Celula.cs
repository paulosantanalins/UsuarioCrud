using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoCelulaRoot.Entity
{
    public class Celula
    {
        public int Id { get; set; }
        public string DescCelula { get; set; }

        public int? IdCelulaPai { get; set; }
        public virtual Celula CelulaPai { get; set; }
        public virtual ICollection<Celula> CelulasSubordinadas { get; set; }


        public virtual ICollection<VinculoCombinadaCelula> VinculoCombinadaCelulas { get; set; }
        public virtual ICollection<VinculoCelulaServico> VinculoCelulaServicos { get; set; }
    }
}
