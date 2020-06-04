using System;

namespace RepasseEAcesso.Domain.SharedRoot.Entity
{
    public class EntityBase
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
