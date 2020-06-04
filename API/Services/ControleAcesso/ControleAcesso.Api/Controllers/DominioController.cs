using ControleAcesso.Domain.DominioRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/dominio")]
    public class DominioController : Controller
    {
        private readonly IDominioService _dominioService;

        public DominioController(
            IDominioService dominioService)
        {
            _dominioService = dominioService;
        }

        [HttpGet("{tipoDominio}")]
        public IActionResult BuscarDominioPorDiscriminator(string tipoDominio)
        {
            var itens = _dominioService.BuscarItens(tipoDominio);
            return Ok(itens);
        }

        [HttpGet("todos/{tipoDominio}")]
        public IActionResult BuscarTodosDominioPorDiscriminator(string tipoDominio)
        {
            var itens = _dominioService.BuscarTodosItens(tipoDominio);
            return Ok(itens);
        }
    }
}