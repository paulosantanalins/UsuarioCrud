namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class ClienteContrato : EntityBase
    {
        public int IdCliente { get; set; }
        public int IdContrato { get; set; }

        public virtual Contrato Contrato { get; set; }
    }
}
