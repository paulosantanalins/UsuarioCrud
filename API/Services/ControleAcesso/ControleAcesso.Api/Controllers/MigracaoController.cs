using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Migracao")]
    public class MigracaoController : Controller
    {
        private readonly IVisualizacaoCelulaService _visualizacaoCelulaService;
        private readonly ICelulaService _celulaService;

        public MigracaoController(
            IVisualizacaoCelulaService visualizacaoCelulaService,
            ICelulaService celulaService)
        {
            _visualizacaoCelulaService = visualizacaoCelulaService;
            _celulaService = celulaService;
        }

        [HttpGet("realizar-migracao/visualizacao-celula/{senha}")]
        public IActionResult RealizarMigracaoVisualizacaoCelula(int senha)
        {
            if (senha == 1234)
            {
                _visualizacaoCelulaService.RealizarMigracaoVisualizacaoCelula();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("realizar-migracao/celula/{senha}")]
        public IActionResult RealizarMigracaoCelula(int senha)
        {
            if (senha == 1234)
            {
                _celulaService.RealizarMigracaoCelulas();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/celula/{senha}")]
        public IActionResult AtualizarMigracaoCelula(int senha)
        {
            try
            {
                if (senha == 1234)
                {
                    _celulaService.AtualizarMigracaoCelulas();
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
    }
}
