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
using GestaoServico.Domain.Dto;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ClassificacaoContabilController : Controller
    {
        protected readonly NotificationHandler _notificationHandler;
        protected readonly IClassificacaoContabilService _classificacaoContabilService;

        public ClassificacaoContabilController(
            NotificationHandler notificationHandler,
            IClassificacaoContabilService classificacaoContabilService)
        {
            _classificacaoContabilService = classificacaoContabilService;
            _notificationHandler = notificationHandler;
        }


        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoViewModel<ClassificacaoContabilVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ClassificacaoContabilDto>>(filtro);
            try
            {
                var resultBD = _classificacaoContabilService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoViewModel<ClassificacaoContabilVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody]ClassificacaoContabilVM model)
        {
            var classificacaoContabil = Mapper.Map<ClassificacaoContabil>(model);
            try
            {
                _classificacaoContabilService.Persistir(classificacaoContabil);
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
                var resultBD = _classificacaoContabilService.BuscarPorId(id);
                var resultVM = Mapper.Map<ClassificacaoContabilVM>(resultBD);
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
                _classificacaoContabilService.Inativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ObterClassificacoes/{id}")]
        public IActionResult carregarClassificacoesPorCategoria([FromRoute] int id)
        {
            try
            {
                var resultBD = _classificacaoContabilService.obterClassificacoesPorCategoria(id);
                var resultVM = Mapper.Map<List<ClassificacaoContabilVM>>(resultBD.ToList());
                return Ok(new { dados = resultVM, notifications = "", success = true });
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
                var resultBD = _classificacaoContabilService.ValidarInativacaoPorId(id);
                return Ok(new { dados = resultBD, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }

}
