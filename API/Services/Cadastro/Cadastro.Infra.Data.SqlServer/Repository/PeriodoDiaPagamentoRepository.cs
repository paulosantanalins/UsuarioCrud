using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class PeriodoDiaPagamentoRepository : BaseRepository<PeriodoDiaPagamento>, IPeriodoDiaPagamentoRepository
    {
        public PeriodoDiaPagamentoRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }
    }
}
