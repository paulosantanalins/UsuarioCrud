using System;

namespace LancamentoFinanceiro.Domain
{
    public class EntityBase
    {
        public int Id { get; set; }
        public DateTime? DtAlteracao { get; set; }
        public string LgUsuario { get; set; }
    }
}
