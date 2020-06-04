using Cadastro.Domain.IntegracaoRoot.DTO;
using Cadastro.Domain.IntegracaoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cadastro.Api.Controllers
{
    [Route("api/[Controller]")]
    [Produces("application/json")]
    public class IntegracaoController : Controller
    {
        private readonly IIntegracaoService _integracaoService;

        public IntegracaoController(
            IIntegracaoService integracaoService)
        {
            _integracaoService = integracaoService;
        }

        [HttpPost("execute")]
        public IActionResult Execute([FromBody] QueryIntegracaoDto queryObject)
        {
            var sucesso = _integracaoService.Execute(queryObject.Query);
            return Ok(sucesso);
        }

        [HttpPost("select")]
        public IActionResult Select([FromBody] QueryIntegracaoDto queryObject)
        {
            var sucesso = _integracaoService.Select(queryObject.Query);
            return Ok(sucesso);
        }

        [HttpPost("selectEacesso")]
        public IActionResult SelectEAcesso([FromBody] QueryIntegracaoDto queryObject)
        {
            var sucesso = _integracaoService.SelectEacesso(queryObject.Query);
            return Ok(sucesso);
        }
    }
}