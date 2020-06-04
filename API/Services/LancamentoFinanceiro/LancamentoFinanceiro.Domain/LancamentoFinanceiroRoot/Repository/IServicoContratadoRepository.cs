using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository
{
    public interface IServicoContratadoRepository : IBaseRepository<ServicoContratado>
    {
        int Validar(ServicoContratado servicoContratado);
    }
}
