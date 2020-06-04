using System.Linq;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class ContratoRepository : BaseRepository<Contrato>, IContratoRepository
    {
        public ContratoRepository(ServiceContext context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
        }

        public int VerificarContratoDefaultPorIdCliente(int idCliente)
        {
            var result = DbSet.FirstOrDefault(x => x.ClientesContratos.Any(y => y.IdCliente == idCliente) && x.NrAssetSalesForce == "");
            if (result != null)
            {
                return result.Id;
            }
            return 0;
        }
    }
}
