using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPortifolioRoot.DTO;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/escopo-servico")]
    public class EscopoServicoController : Controller
    {
        private readonly IEscopoServicoService _escopoServicoService;
        private readonly NotificationHandler _notificationHandler;
        public EscopoServicoController(IEscopoServicoService escopoServicoService,
                                       NotificationHandler notificationHandler)
        {
            _escopoServicoService = escopoServicoService;
            _notificationHandler = notificationHandler;
        }

        [HttpGet]
        public IActionResult FiltrarRepasse(FiltroGenericoViewModelBase<GridEscopoVM> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDto<GridEscopoDTO>>(filtroVM);
                var result = _escopoServicoService.Filtrar(filtroDto);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("filtrar")]
        public IActionResult FiltrarRepassePost([FromBody] FiltroGenericoViewModelBase<GridEscopoVM> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDto<GridEscopoDTO>>(filtroVM);
                var result = _escopoServicoService.Filtrar(filtroDto);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idEscopo}")]
        public IActionResult ObterPorId(int idEscopo)
        {
            try
            {
                var resultBD = Mapper.Map<EscopoVM>(_escopoServicoService.BuscarPorId(idEscopo));
                return Ok((new { dados = resultBD, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        [HttpGet("{id}/verificar-inativacao-valida")]
        public IActionResult VerificarExclusaoValida([FromRoute] int id)
        {
            try
            {
                var result = _escopoServicoService.VerificarExclusaoValida(id);
                return Ok(new
                {
                    dados = result,
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("buscar-ativos")]
        public IActionResult BuscarApenasAtivos()
        {
            try
            {
                var resultBD = _escopoServicoService.BuscarAtivos();
                return Ok(new
                {
                    dados = resultBD,
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody]EscopoVM escopoVM)
        {
            try
            {
                var escopo = Mapper.Map<EscopoServico>(escopoVM);
                _escopoServicoService.PersistirEscopoServico(escopo);
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

        [HttpPut]
        public IActionResult Atualizar([FromBody]EscopoVM escopoVM)
        {
            try
            {
                var escopo = Mapper.Map<EscopoServico>(escopoVM);
                _escopoServicoService.PersistirEscopoServico(escopo);
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

        [HttpPut]
        [Route("{idEscopo}/alterar-status")]
        public IActionResult AlterarStatusEscopoServico(int idEscopo)
        {
            try
            {
                _escopoServicoService.AlterarStatus(idEscopo);
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


        [HttpGet("{idPortfolio}/buscar-por-portfolio")]
        public IActionResult BuscarPorPortfolio(int idPortfolio)
        {
            try
            {
                var resultBD = _escopoServicoService.BuscarAtivosPorPortfolio(idPortfolio);
                return Ok(new
                {
                    dados = resultBD,
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


    }
}
