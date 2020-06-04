using System;
using AutoMapper;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ReajusteContratoController : Controller
    {
        private readonly IReajusteContratoService _reajusteContratoService;
        private readonly IMapper _mapper;

        public ReajusteContratoController(IReajusteContratoService reajusteContratoService, IMapper mapper)
        {
            _reajusteContratoService = reajusteContratoService;
            _mapper = mapper;
        }

        [HttpGet("obter-periodos")]
        public IActionResult CarregarComboPeriodo()
        {
            try
            {
                var periodos = _reajusteContratoService.BuscarPeriodosComFinalizacaoCadastradas();
                return Ok(periodos);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult FiltrarTransferencias([FromBody] FiltroComPeriodo<ReajusteContratoGridDto> filtro)
        {
            try
            {
                var result = _reajusteContratoService.Filtrar(filtro);
                return Ok(new { dados = result, notifications = "", success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-prestadores-celula/{idCelula}")]
        public IActionResult ObterPrestadoresPorCelula([FromRoute]int idCelula)
        {
            try
            {
                var result = _reajusteContratoService.ObterPrestadoresPorCelula(idCelula);
                return Ok(new { dados = result, notifications = "", success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-valores-atuais/{idPrestador}/{filtrar}")]
        public IActionResult ObterValoresAtuais([FromRoute] int idPrestador, [FromRoute] bool filtrar)
        {
            try
            {
                var valores = _reajusteContratoService.ObterValoresAtuais(idPrestador, filtrar);
                return Ok(new { dados = valores.ValoresContratoPrestadorModel, notifications = valores.Message, success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-logs/{id}")]
        public IActionResult ObterLogsFinalizacoes([FromRoute] int id)
        {
            try
            {
                var logs = _reajusteContratoService.ObterLogsPorId(id);
                return Ok(logs);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPost("reajustar-contrato")]
        public IActionResult FinalizarContrato([FromBody] ValoresContratoPrestadorModel valoresContratoPrestador)
        {
            try
            {
                var finalizacaoContrato = _mapper.Map<ReajusteContrato>(valoresContratoPrestador);
                _reajusteContratoService.ReajustarContrato(finalizacaoContrato);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("consultar-reajuste/{id}")]
        public IActionResult ConsultarFinalizacao([FromRoute] int id)
        {
            try
            {
                var result = _reajusteContratoService.ConsultarReajuste(id);
                return Ok(new { dados = result, notifications = string.Empty, success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPut("aprovar/{id}")]
        public IActionResult AprovarReajuste([FromRoute] int id)
        {
            try
            {
                _reajusteContratoService.AprovarReajuste(id);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPut("negar")]
        public IActionResult NegarReajuste([FromBody] InativacaoFinalizacaoContratoDto inativacao)
        {
            try
            {
                _reajusteContratoService.NegarReajute(inativacao);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }
    }
}
