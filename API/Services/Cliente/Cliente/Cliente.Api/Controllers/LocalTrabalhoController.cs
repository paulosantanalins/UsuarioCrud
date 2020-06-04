using Cliente.Domain.ClienteRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cliente.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/localtrabalho")]
    public class LocalTrabalhoController : Controller
    {
        private readonly IClienteService _clienteService;

        public LocalTrabalhoController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        [Route("localeacesso/{idCliente}")]
        public async Task<IActionResult> LocalTrabalhoIdClienteEAcesso([FromRoute] int idCliente)
        {
            try
            {
                var resultStfCorp = _clienteService.ObterLocalTrabalhoIdClienteEAcesso(idCliente);
                return Ok(resultStfCorp);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{idLocalTrabalho}")]
        public IActionResult BuscarLocalTrabalhoPorId([FromRoute] int idLocalTrabalho)
        {
            try
            {
                var result = _clienteService.ObterLocalTrabalhoEacesso(idLocalTrabalho);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
