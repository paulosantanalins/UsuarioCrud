using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Cadastro.Infra.CrossCutting.IoC;

namespace Cadastro.Api.Jobs
{
    public class AtualizarMigracaoPessoaJob
    {
        public void Execute()
        {
            var _pessoaService = RecuperarPessoaService();
            _pessoaService.AtualizarMigracao();
        }

        private static IPessoaService RecuperarPessoaService()
        {
            var pessoaService = Injector.ServiceProvider.GetService(typeof(IPessoaService)) as IPessoaService;
            return pessoaService;
        }
    }
}
