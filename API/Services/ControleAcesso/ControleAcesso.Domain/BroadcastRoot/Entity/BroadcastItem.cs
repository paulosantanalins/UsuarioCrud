using System.Collections.Generic;

namespace ControleAcesso.Domain.BroadcastRoot.Entity
{
    public class BroadcastItem : EntityBase
    {
        public ICollection<Broadcast> Broadcasts { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }
}

