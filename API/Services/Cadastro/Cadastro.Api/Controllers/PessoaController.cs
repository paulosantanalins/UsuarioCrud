using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Pessoa")]
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;
        private readonly IViewProfissionaisService _viewProfissionaisService;

        public PessoaController(
            IPessoaService pessoaService, IViewProfissionaisService viewProfissionaisService)
        {
            _pessoaService = pessoaService;
            _viewProfissionaisService = viewProfissionaisService;
        }

        [HttpGet("idEacesso/{idEacesso}")]
        public IActionResult BuscarIdPorIdEAcesso(int idEacesso)
        {
            var idPessoa = _pessoaService.ObterIdPessoa(idEacesso);
            return Ok(idPessoa);
        }

        [HttpGet("obter-por-id/{id}")]
        public IActionResult BuscarPessoaPorId(int id)
        {
            var pessoa = _pessoaService.ObterPessoa(id);
            return Ok(pessoa);
        }

        [HttpGet("obter-por-email-interno/{email}")]
        public IActionResult BuscarPessoaPorEmail(string email)
        {
            var pessoa = _pessoaService.BuscarPorEmailInterno(email);
            return Ok(pessoa);
        }

        [HttpGet("obter-por-idEAcesso/{idEAcesso}")]
        public IActionResult BuscarPessoaPorIdEAcesso(int idEAcesso)
        {
            var pessoa = _pessoaService.BuscarPorIdEAcesso(idEAcesso);
            return Ok(pessoa);
        }

        [HttpGet("view-profissinal-geral-eacesso")]
        public IActionResult ViewProfissionalGeralEacesso()
        {
            var result = _viewProfissionaisService.ViewProfissionalGeralEacesso();
            return Ok(result);
        }

        [HttpGet("view-profissionais-desligados-30-dias-eacesso")]
        public IActionResult ViewProfissionaisDesligados30DiasEacesso()
        {
            var result = _viewProfissionaisService.ViewProfissionaisDesligados30Dias();
            return Ok(result);
        }

        [HttpGet("view-adimitidos-futuros-natcorp")]
        public IActionResult ViewAdimitidosFuturosNatCorp()
        {
            var result = _viewProfissionaisService.ViewAdimitidosFuturosNatCorp();
            return Ok(result);
        }

        [HttpGet("view-inativados-natcorp")]
        public IActionResult ViewInativadosNatCorp()
        {
            var result = _viewProfissionaisService.ViewInativadosNatCorp();
            return Ok(result);
        }

        [HttpGet("view-demissoes-futuras-natcorp")]
        public IActionResult ViewDemissaoFuturas()
        {
            var result = _viewProfissionaisService.ViewDemissaoFuturas();
            return Ok(result);
        }
    }
}
