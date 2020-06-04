using Cadastro.Domain.FilialRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FilialController : Controller
    {
        protected readonly IFilialService _filialService;

        public FilialController(
            IFilialService filialService)
        {
            _filialService = filialService;
        }

        [HttpGet("rm/{idColigada}")]
        public IActionResult BuscarNoRM([FromRoute] int idColigada)
        {
            var result = _filialService.BuscarNoRm(idColigada);
            return Ok(result);
        }
    }
}
