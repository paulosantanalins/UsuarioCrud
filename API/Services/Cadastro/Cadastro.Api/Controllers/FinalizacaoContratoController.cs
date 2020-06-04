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
    public class FinalizacaoContratoController : Controller
    {
        private readonly IFinalizacaoContratoService _finalizacaoContratoService;
        private readonly IMapper _mapper;

        public FinalizacaoContratoController(IFinalizacaoContratoService finalizacaoContratoService, IMapper mapper)
        {
            _finalizacaoContratoService = finalizacaoContratoService;
            _mapper = mapper;
        }

        [HttpGet("obter-periodos")]
        public IActionResult CarregarComboPeriodo()
        {
            try
            {
                var periodos = _finalizacaoContratoService.BuscarPeriodosComFinalizacaoCadastradas();
                return Ok(periodos);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult FiltrarTransferencias([FromBody] FiltroComPeriodo<FinalizarContratoGridDto> filtro)
        {
            try
            {
                var result = _finalizacaoContratoService.Filtrar(filtro);
                return Ok(new { dados = result, notifications = "", success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-prestadores-celula/{idCelula}/{filtrar}")]
        public IActionResult ObterPrestadoresPorCelula([FromRoute]int idCelula, [FromRoute] bool filtrar)
        {
            try
            {
                var result = _finalizacaoContratoService.ObterPrestadoresPorCelula(idCelula, filtrar);
                return Ok(new { dados = result, notifications = "", success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPost("finalizar-contrato")]
        public IActionResult FinalizarContrato([FromBody] FinalizacaoContratoDto finalizacaoContratoDto)
        {
            try
            {
                var finalizacaoContrato = _mapper.Map<FinalizacaoContrato>(finalizacaoContratoDto);
                _finalizacaoContratoService.FinalizarContrato(finalizacaoContrato, finalizacaoContratoDto);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("consultar-finalizacao/{id}")]
        public IActionResult ConsultarFinalizacao([FromRoute] int id)
        {
            try
            {
                var result = _finalizacaoContratoService.ConsultarFinalizacao(id);
                return Ok(new { dados = result, notifications = string.Empty, success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPut("editar")]
        public IActionResult EditarFinalizacao([FromBody] FinalizacaoContratoDto finalizacaoContratoDto)
        {
            try
            {
                var finalizacaoContrato = _mapper.Map<FinalizacaoContrato>(finalizacaoContratoDto);
                _finalizacaoContratoService.EditarFinalizacao(finalizacaoContrato,
                    finalizacaoContratoDto.FinalizarImediatamente);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPut("inativar")]
        public IActionResult InativarFinalizacao([FromBody] InativacaoFinalizacaoContratoDto inativacaoFinalizacaoContratoDto)
        {
            try
            {
                _finalizacaoContratoService.InativarFinalizacao(inativacaoFinalizacaoContratoDto);
                return Ok();
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
                var logs = _finalizacaoContratoService.ObterLogsPorId(id);
                return Ok(logs);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("efetivar")]
        public IActionResult ExecutarJogFinalizacaoContratos()
        {
            try
            {
                _finalizacaoContratoService.EfetuarFinalizacoes();
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }
    }
}
