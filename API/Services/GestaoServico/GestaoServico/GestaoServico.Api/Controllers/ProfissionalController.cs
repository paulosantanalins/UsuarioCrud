using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Domain.Core.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Profissional")]
    public class ProfissionalController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ProfissionalController(NotificationHandler notificationHandler, IOptions<ConnectionStrings> connectionStrings)
        {
            _notificationHandler = notificationHandler;
            _connectionStrings = connectionStrings;
        }

        [HttpGet("ObterTopProfissionaisPorSituacao/{situacao}")]
        public IActionResult Obtertodos(int situacao)
        {
            try
            {
                var profissionalEacessoService = new ProfissionalEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = profissionalEacessoService.ObterProfissionais(situacao);
                var resultVM = Mapper.Map<List<MultiselectViewModel>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ObterProfissionaisPorSituacaoPorFiltro/{situacao}/{filtro}")]
        public IActionResult Obtertodos(int situacao, string filtro)
        {
            try
            {
                var profissionalEacessoService = new ProfissionalEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = profissionalEacessoService.ObterProfissionaisFiltrados(filtro, situacao);
                var resultVM = Mapper.Map<List<MultiselectViewModel>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ObterProfissionaisPorId/{id}")]
        public IActionResult ObterPorId(int id)
        {
            try
            {
                var profissionalEacessoService = new ProfissionalEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = profissionalEacessoService.ObterProfissionalPorId(id);
                var resultVM = Mapper.Map<ProfissionalEAcessoMultiSelectVM>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ObterValorProfissionalPorId/{id}")]
        public IActionResult ObterValorProfissionalPorId(int id)
        {
            try
            {
                var profissionalEacessoService = new ProfissionalClienteEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = profissionalEacessoService.ObterValorCustoProfissional(id);
                return Ok(new { dados = resultBD, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ObterProfissionalPorCelula/{situacao}/{idCelula}")]
        public IActionResult ObterProfissionalPorCelula(int situacao, int idCelula)
        {
            try
            {
                var profissionalEacessoService = new ProfissionalEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = profissionalEacessoService.ObterProfissionalPorCelula(situacao, idCelula);
                return Ok(new { dados = resultBD, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


    }
}
