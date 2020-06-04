using AutoMapper;
using Forecast.Api.ViewModels;
using Forecast.Domain.ForecastRoot;
using Forecast.Domain.ForecastRoot.Dto;
using Forecast.Domain.ForecastRoot.Service.Interfaces;
using Forecast.Domain.SharedRoot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Utils;
using Utils.Base;
using Utils.Calendario;

namespace Forecast.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ForecastController : Controller
    {
        private readonly IForecastService _forecastService;
        private readonly ICalendarioService _calendarioService;
        private readonly IVariablesToken _variables;

        public ForecastController(IForecastService forecastService, ICalendarioService calendarioService, IVariablesToken variables)
        {
            _forecastService = forecastService;
            _calendarioService = calendarioService;
            _variables = variables;
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoViewModelBase<ForecastVM> filtro)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDtoBase<ForecastDto>>(filtro);
                var resultBD = _forecastService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoViewModelBase<ForecastDto>>(resultBD);

                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("obter-quantidade-dias-apos-virada-mes")]
        public IActionResult ValidarDoisDiasViradaMes()
        {
            try
            {
                var dias = _forecastService.ObterQuantidadeDiasUteisAposViradaMes();
                return Ok(dias);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            try
            {
                var resultBD = _forecastService.BuscarPorId(id);

                var resultVM = Mapper.Map<ForecastVM>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{idCelula}/{idCliente}/{idServico}/{nrAno}")]
        public IActionResult BuscarPorIdComposto([FromRoute] int idCelula, int idCliente, int idServico, int nrAno)
        {
            try
            {
                var resultBD = _forecastService.BuscarPorIdComposto(idCelula, idCliente, idServico, nrAno);

                return Ok(resultBD);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] ForecastVM forecastVM)
        {
        
            try
            {
                var forecast = Mapper.Map<ForecastET>(forecastVM);
                forecast.ValorForecast.Usuario = _variables.UsuarioToken;

                if (forecastVM.Id == 0)
                    _forecastService.Adicionar(forecast);
                else if(forecastVM.Id == 1)
                    _forecastService.Atualizar(forecast);

                return Ok(true);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(409, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("todos-anos")]
        public IActionResult BuscarTodosAnos()
        {
            try
            {
                var resultBD = _forecastService.BuscarTodosAnos();

                var resultVM = Mapper.Map<IEnumerable<int>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("eacesso/{idCelula}/{idCliente}")]
        public IActionResult GetServicoPorIdCelulaIdClienteEAcesso([FromRoute]int idCelula, [FromRoute]int idCliente)
        {
            try
            {
                var resultStfCorp = _forecastService.ObterServicoPorIdCelulaIdClienteEAcesso(idCelula, idCliente);
                return Ok(resultStfCorp);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpPost("verificar-registro-forecast-ano")]
        public IActionResult VerificarSeRegistroForecastExiste([FromBody] ForecastVM forecastVM)
        {        
            try
            {
                var forecast = Mapper.Map<ForecastET>(forecastVM);
                forecast.ValorForecast.Usuario = _variables.UsuarioToken;
                var resultStfCorp = _forecastService.VerificarSeRegistroExiste(forecast);
                return Ok(resultStfCorp);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
