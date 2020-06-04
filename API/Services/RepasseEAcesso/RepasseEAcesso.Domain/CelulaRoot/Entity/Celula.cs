using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Entity;
using System.Collections.Generic;

namespace RepasseEAcesso.Domain.CelulaRoot.Entity
{
    public class Celula : EntityBase
    {
        public string DescCelula { get; set; }
        public bool FlHabilitarRepasseMesmaCelula { get; set; }
        public bool FlHabilitarRepasseEpm { get; set; }

        public ICollection<RepasseNivelUm> RepassesOrigem { get; set; }
        public ICollection<RepasseNivelUm> RepassesDestino { get; set; }
    }
}
