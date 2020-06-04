using FluentScheduler;

namespace GestaoServico.Api.Jobs.Base
{
    public class JobRegistry : Registry
    {
        public JobRegistry()
        {
            
            NonReentrantAsDefault();
            //Schedule<ObterContratosJob>().ToRunNow();
            //Schedule<ObterContratosJob>().ToRunNow().AndEvery(30).Minutes();
            //Schedule<RepasseEPMJob>().ToRunEvery(1).Months().On(24).At(23, 59);
            //Schedule<RepasseEPMJob>().ToRunNow();
            //Schedule<MigrarServicosEacessoJob>().ToRunNow().AndEvery(240).Minutes();
            //Schedule<RepasseEAcessoJob>().ToRunNow();
        }
    }
}
