using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface IContratoRepository : IBaseRepository<Contrato>
    {
        int VerificarContratoDefaultPorIdCliente(int idCliente);
    }
}
