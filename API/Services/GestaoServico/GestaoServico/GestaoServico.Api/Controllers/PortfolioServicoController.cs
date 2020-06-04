using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/PortfolioServico")]
    public class PortfolioServicoController : Controller
    {
        private readonly IPortfolioServicoService _portfolioServicoService;
        private readonly NotificationHandler _notificationHandler;
        private readonly Variables _variables;

        public PortfolioServicoController(NotificationHandler notificationHandler,
                IPortfolioServicoService portfolioServicoService,
                Variables variables)
        {
            _portfolioServicoService = portfolioServicoService;
            _notificationHandler = notificationHandler;
            _variables = variables;
        }


        [HttpPost]
        public IActionResult Persistir([FromBody]PortfolioServicoVM portfolioServicoVM)
        {
            var portfolio = Mapper.Map<PortfolioServico>(portfolioServicoVM);
            try
            {
                _portfolioServicoService.Persistir(portfolio);
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

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoViewModel<PortfolioServicoVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<PortfolioServicoDto>>(filtro);
            try
            {
                var resultBD = _portfolioServicoService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoViewModel<PortfolioServicoVM>>(resultBD);

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
                _portfolioServicoService.Inativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ValidarInexistencia/{id}")]
        public IActionResult ValidarInexistencia([FromRoute] int id)
        {
            try
            {
                var resultBD = _portfolioServicoService.ValidarInexistencia(id);
                return Ok(new { dados = resultBD, notifications = "", success = true });
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
                var resultBD = _portfolioServicoService.BuscarPorId(id);
                var resultVM = Mapper.Map<PortfolioServicoVM>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public IActionResult Obtertodos()
        {
            try
            {
                var resultBD = _portfolioServicoService.ObterTodos();
                var resultVM = Mapper.Map<IEnumerable<PortfolioServicoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-ativos")]
        public IActionResult ObterAtivos()
        {
            try
            {
                var resultBD = _portfolioServicoService.ObterTodosAtivos();
                var resultVM = Mapper.Map<IEnumerable<PortfolioServicoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
