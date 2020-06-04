using System.Linq;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class ClienteRepository : BaseRepository<ClienteET>, IClienteRepository
    {
        public ClienteRepository(ServiceContext context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
        }

        public int VerificarIdCliente()
        {
            var result = DbSet.Where(x => x.Id >= 20000).Count();
            return result;
        }
    }
}
