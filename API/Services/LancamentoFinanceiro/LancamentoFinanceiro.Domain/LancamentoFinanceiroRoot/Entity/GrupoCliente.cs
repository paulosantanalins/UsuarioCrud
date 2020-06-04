using System.Collections.Generic;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class GrupoCliente : EntityBase
    {
        public string DescGrupoCliente { get; set; }
        public bool FlStatus { get; set; }
        public string IdClienteMae { get; set; }
        public virtual ICollection<ClienteET> Clientes { get; set; }
    }
}
