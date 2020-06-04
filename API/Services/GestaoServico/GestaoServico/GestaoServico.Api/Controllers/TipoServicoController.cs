using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/TipoServico")]
    public class TipoServicoController : Controller
    {
        private readonly ITipoServicoService _tipoServicoService;
        private readonly NotificationHandler _notificationHandler;
        private readonly Utils.Variables _variables;

        public TipoServicoController(NotificationHandler notificationHandler,
                                     Utils.Variables variables,
                                     ITipoServicoService tipoServicoService)
        {
            _tipoServicoService = tipoServicoService;
            _notificationHandler = notificationHandler;
            _variables = variables;
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoViewModel<TipoServicoVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<TipoServico>>(filtro);
            try
            {
                var resultBD = _tipoServicoService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<TipoServicoVM>>(resultBD);

                //Request.HttpContext.Response.Headers.Add("X-Total-Count", "10");
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                //Request.HttpContext.Response.Headers.Add("X-Total-Count", "10");
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody]TipoServicoVM tipoServicoVM)
        {
            try
            {
                var tipoServico = Mapper.Map<TipoServico>(tipoServicoVM);
                _tipoServicoService.Persistir(tipoServico);
                return Ok(new
                {
                    dados = "",
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            try
            {
                var resultBD = _tipoServicoService.BuscarPorId(id);
                var resultVM = Mapper.Map<TipoServico>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Inativar")]
        public IActionResult Inativar([FromBody] int id)
        {
            try
            {
                _tipoServicoService.Inativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public IActionResult BuscarTodos()
        {
            try
            {
                var resultBD = _tipoServicoService.BuscarTodos();
                var resultVM = Mapper.Map<IEnumerable<TipoServico>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("VerificarExistencia/{id}")]
        public IActionResult VerificarExisteciaPorId([FromRoute] int id)
        {
            try
            {
                var resultBD = _tipoServicoService.VerificarExisteciaPorId(id);
                return Ok(new { dados = resultBD, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("BuscarTodosAtivos")]
        public IActionResult BuscarTodosAtivos()
        {
            try
            {
                var resultBD = _tipoServicoService.BuscarTodosAtivos();
                var resultVM = Mapper.Map<IEnumerable<TipoServico>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
