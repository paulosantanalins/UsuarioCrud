using System.Linq;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace LancamentoFinanceiro.Infra.Data.SqlServer.Repository
{
    public class ServicoContratadoRepository : BaseRepository<ServicoContratado>, IServicoContratadoRepository
    {
        public ServicoContratadoRepository(ServiceContext context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

        public int Validar(ServicoContratado servicoContratado)
        {
            var result = DbSet.FirstOrDefault(x => (x.Id != servicoContratado.Id) &&
                                                   (x.IdContrato == servicoContratado.IdContrato) &&
                                                   (x.IdEscopoServico == servicoContratado.IdEscopoServico) &&
                                                   (x.IdEmpresa == servicoContratado.IdEmpresa) &&
                                                   (x.IdFilial == servicoContratado.IdFilial));
            return result?.Id ?? 0;
        }
    }
}
