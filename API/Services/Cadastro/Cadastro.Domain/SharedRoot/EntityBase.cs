using System;

namespace Cadastro.Domain.SharedRoot
{
    public class EntityBase
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
