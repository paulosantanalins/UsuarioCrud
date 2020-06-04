using AutoMapper;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TransferenciaPrestadorController : Controller
    {
        private readonly ITransferenciaPrestadorService _transferenciaPrestadorService;

        public TransferenciaPrestadorController(ITransferenciaPrestadorService transferenciaPrestadorService)
        {
            _transferenciaPrestadorService = transferenciaPrestadorService;
        }

        [HttpGet("obter-prestadores-celula/{idCelula}")]
        public IActionResult ObterPrestadoresPorCelula([FromRoute]int idCelula)
        {
            try
            {
                var result = _transferenciaPrestadorService.ObterPrestadoresPorCelula(idCelula);
                return Ok(new { dados = result, notifications = "", success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-prestador-transferencia/{idPrestador}")]
        public IActionResult ObterPrestadorParaTransferencia([FromRoute] int idPrestador)
        {
            try
            {
                var result = _transferenciaPrestadorService.ObterPrestadorParaTransferencia(idPrestador, true);
                return Ok(new
                {
                    dados = result.PrestadorParaTransferenciaDto, notifications = result.Message,
                    success = true
                });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-origem-transferencia/{idPrestador}")]
        public IActionResult ObterOrigemTransferencia([FromRoute] int idPrestador)
        {
            try
            {
                var result = _transferenciaPrestadorService.ObterPrestadorParaTransferencia(idPrestador, false);
                return Ok(new {dados = result.PrestadorParaTransferenciaDto, notifications = string.Empty, success = true});
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("consultar-transferencia/{idTransferencia}")]
        public IActionResult ConsultarTransferencia([FromRoute] int idTransferencia)
        {
            try
            {
                var result = _transferenciaPrestadorService.ConsultarTransferencia(idTransferencia);
                return Ok(new { dados = result, notifications = string.Empty, success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPost("solicitar-transferencia")]
        public IActionResult SolicitarTransferenciaPrestador([FromBody]PrestadorParaTransferenciaDto prestadorParaTransferenciaDto)
        {
            try
            {
                var solicitarTransferencia = Mapper.Map<TransferenciaPrestador>(prestadorParaTransferenciaDto);
                var result = _transferenciaPrestadorService.SolicitarTransferenciaPrestador(solicitarTransferencia);
                return Ok(new { success = result == string.Empty, message = result });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPut("editar-transferencia")]
        public IActionResult EditarTransferencia([FromBody] PrestadorParaTransferenciaDto prestadorParaTransferenciaDto)
        {
            try
            {
                var solicitarTransferencia = Mapper.Map<TransferenciaPrestador>(prestadorParaTransferenciaDto);
                var result = _transferenciaPrestadorService.AtualizarTransferenciaPrestador(solicitarTransferencia);
                return Ok(new { success = result == string.Empty, message = result });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult FiltrarTransferencias([FromBody] FiltroComPeriodo<TransferenciaPrestadorDto> filtro)
        {
            try
            {
                var result = _transferenciaPrestadorService.FiltrarTransferencia(filtro);
                return Ok(new { dados = result, notifications = "", success = true });
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }



        [HttpGet("obter-periodos")]
        public IActionResult CarregarComboPeriodo()
        {
            try
            {
                var periodos = _transferenciaPrestadorService.BuscarPeriodosComTransferenciasCadastradas();
                return Ok(periodos);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }


        [HttpPut("aprovar")]
        public IActionResult AprovarTransferencia([FromBody] AprovarTransferenciaPrestadorDto idTransf)
        {            
            try
            {                
                _transferenciaPrestadorService.AprovarTransferencia(idTransf);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpPut("negar")]
        public IActionResult NegarTransferencia([FromBody] NegarTransferenciaPrestadorDto negacaoDto)
        {
            try
            { 
                _transferenciaPrestadorService.NegarTransferencia(negacaoDto);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }

        [HttpGet("obter-logs-transferencia")]
        public IActionResult ObterLogsTransferencia(int idTransf)
        {
            try
            {
                var logs = _transferenciaPrestadorService.ObterLogsTransferencia(idTransf);
                return Ok(logs);
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }
        }
        
        [HttpGet("efetivar")]
        public IActionResult EfetivarTransferenciaPrestadores()
        {
            try
            {
                _transferenciaPrestadorService.EfetivarTransferenciasDePrestadoresJob();
                return Ok();
            }
            catch (Exception exception) 
            {
                return BadRequest(exception);
            }
        }

    }
}
