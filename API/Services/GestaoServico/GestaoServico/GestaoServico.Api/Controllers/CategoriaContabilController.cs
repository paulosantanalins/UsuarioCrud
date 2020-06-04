using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Domain.Core.Notifications;
using Utils;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CategoriaContabilController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly ICategoriaContabilService _categoriaContabilService;

        public CategoriaContabilController(NotificationHandler notificationHandler, ICategoriaContabilService categoriaContabilService)
        {
            _notificationHandler = notificationHandler;
            _categoriaContabilService = categoriaContabilService;
        }

        [HttpGet]
        public IActionResult BuscarTodas()
        {
            try
            {
                var resultBD = _categoriaContabilService.BuscarTodas();
                var resultVM = Mapper.Map<IEnumerable<CategoriaContabilVM>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Ativas")]
        public IActionResult BuscarAtivas()
        {
            try
            {
                var resultBD = _categoriaContabilService.BuscarTodasAtivas();
                var resultVM = Mapper.Map<IEnumerable<CategoriaContabilVM>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoViewModel<CategoriaContabilVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<CategoriaContabil>>(filtro);
            try
            {
                var resultBD = _categoriaContabilService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<CategoriaContabilVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody]CategoriaContabilVM categoriaVM)
        {
            var categoria = Mapper.Map<CategoriaContabil>(categoriaVM);
            try
            {
                _categoriaContabilService.Persistir(categoria);
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
                var resultBD = _categoriaContabilService.BuscarPorId(id);
                var resultVM = Mapper.Map<CategoriaContabilVM>(resultBD);

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
                _categoriaContabilService.Inativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ValidarInativacao/{id}")]
        public IActionResult ValidarInativacaoPorId([FromRoute] int id)
        {
            try
            {
                var resultBD = _categoriaContabilService.ValidarInativacaoPorId(id);
                return Ok(new { dados = resultBD, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
