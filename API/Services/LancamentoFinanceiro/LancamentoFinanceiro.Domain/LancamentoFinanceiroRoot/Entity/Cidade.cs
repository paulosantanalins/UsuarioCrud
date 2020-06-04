using System.Collections.Generic;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class Cidade : EntityBase
    {
        public string NmCidade { get; set; }

        public int IdEstado { get; set; }

        public virtual ICollection<Endereco> Enderecos { get; set; }
    }
}
