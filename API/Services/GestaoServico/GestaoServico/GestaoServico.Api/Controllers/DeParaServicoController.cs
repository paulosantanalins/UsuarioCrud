using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using Utils.Base;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/de-para")]
    public class DeParaServicoController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IDeParaServicoService _deParaServicoService;

        public DeParaServicoController(NotificationHandler notificationHandler,
                                       IDeParaServicoService deParaServicoService)
        {
            _notificationHandler = notificationHandler;
            _deParaServicoService = deParaServicoService;
        }

        [HttpGet]
        public IActionResult FiltrarServicosMigrados([FromQuery]FiltroGenericoViewModelBase<GridServicoMigradoVM> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDtoBase<GridServicoMigradoDTO>>(filtroVM);
                var result = Mapper.Map<FiltroGenericoViewModelBase<GridServicoMigradoVM>>(_deParaServicoService.Filtrar(filtroDto).Result);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("filtrar")]
        public IActionResult FiltrarServicosMigradosPost([FromBody]FiltroGenericoViewModelBase<GridServicoMigradoVM> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDtoBase<GridServicoMigradoDTO>>(filtroVM);
                var result = Mapper.Map<FiltroGenericoViewModelBase<GridServicoMigradoVM>>(_deParaServicoService.Filtrar(filtroDto).Result);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }



        [HttpGet("{id}")]
        public IActionResult ObterServicoMigradoPorId(int id)
        {
            try
            {
                var resultDb = _deParaServicoService.ObterServicoMigradoPorId(id);
                var result = Mapper.Map<DeParaViewModel>(resultDb);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idServicoEacesso}/obter-servico-contratado-por-servico-eacesso")]
        public IActionResult ObterIdServicoContratadoPorIdSericoEacesso(int idServicoEacesso)
        {
            try
            {
                var result = _deParaServicoService.BuscarIdServicoContratadoPorIdServicoEacesso(idServicoEacesso);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("migrar-servicos")]
        public IActionResult MigrarServicos()
        {
            _deParaServicoService.MigrarServicos();
            return Ok();
        }

        [HttpGet("migrar-movimentacao")]
        public IActionResult MigrarMovimentacao()
        {
            _deParaServicoService.MigrarMovimentacao();
            return Ok();
        }

    }
}
