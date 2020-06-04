using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using Utils.StfAnalitcsDW.Model;

namespace LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface
{
    public interface IServicoContratadoService
    {
        int PersistirServicoEacesso(ViewServicoModel viewServico);
    }
}
