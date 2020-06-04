using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Cadastro.Infra.CrossCutting.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.Jobs
{
    public class MigrarProfissionaisNatcorpJob
    {
        public void Execute()
        {
            var _pessoaService = RecuperarPrestadorService();
            _pessoaService.RealizarMigracaoProfissionaisNatcorp(DateTime.Now.Date);
        }

        private static IPessoaService RecuperarPrestadorService()
        {
            var prestadorService = Injector.ServiceProvider.GetService(typeof(IPessoaService)) as IPessoaService;
            return prestadorService;
        }
    }
}
