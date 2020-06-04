using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface IClienteRepository : IBaseRepository<ClienteET>
    {
        int VerificarIdCliente();
    }
}
