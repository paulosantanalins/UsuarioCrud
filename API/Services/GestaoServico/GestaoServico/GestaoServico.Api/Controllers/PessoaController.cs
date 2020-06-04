using GestaoServico.Domain.PessoaRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/pessoa")]
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;

        public PessoaController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        [HttpGet("migrar")]
        public IActionResult Migrar()
        {
            _pessoaService.Migrar();
            return Ok();
        }

        [HttpGet("atualizar-migracao")]
        public IActionResult AtualizarMigracao()
        {
            _pessoaService.AtualizarMigracao();
            return Ok();
        }
    }
}
