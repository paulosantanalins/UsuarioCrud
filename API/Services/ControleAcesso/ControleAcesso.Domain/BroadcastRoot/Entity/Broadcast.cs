using System;

namespace ControleAcesso.Domain.BroadcastRoot.Entity
{
    public class Broadcast : EntityBase
    {
        public virtual BroadcastItem BroadcastItem { get; set; }
        public int IdBroadcastItem { get; set; }
        public bool Excluido { get; set; }
        public bool Lido { get; set; }
        public string LgUsuarioVinculado { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}