using FluentScheduler;

namespace LancamentoFinanceiro.Api.Job.Base
{
    public class JobRegistry: Registry
    {
        public JobRegistry()
        {
            NonReentrantAsDefault();
            //Schedule<LancamentoFinanceiroJob>().ToRunNow();
        }
    }
}
