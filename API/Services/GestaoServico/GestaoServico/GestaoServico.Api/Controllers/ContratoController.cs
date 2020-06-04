using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ContratoController : Controller
    {
        protected readonly NotificationHandler _notificationHandler;
        protected readonly IContratoService _contratoService;
        protected readonly IServicoContratadoService _servicoContratadoService;

        public ContratoController(NotificationHandler notificationHandler, IContratoService contratoService, IServicoContratadoService servicoContratadoService)
        {
            notificationHandler = _notificationHandler;
            _contratoService = contratoService;
            _servicoContratadoService = servicoContratadoService;
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoViewModel<ContratoVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ContratoDto>>(filtro);
            try
            {
                var resultBD = _contratoService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<ContratoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult PersistirInformacoesContrato([FromBody]InformacoesContratoVM informacoesContrato)
        {
            try
            {
                _contratoService.PersistirInformacoesContrato(informacoesContrato.Id, informacoesContrato.IdMoeda, informacoesContrato.IdCelulaComercial, informacoesContrato.DtInicial, informacoesContrato.DtFinalizacao);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { dados = ex, notifications = "", success = false });
            }
        }

        [HttpGet]
        public IActionResult Obtertodos()
        {
            try
            {
                var resultBD = _contratoService.ObterTodos();
                var resultVM = Mapper.Map<IEnumerable<ContratoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("popular-combo")]
        public IActionResult ObtertodosCombo()
        {
            try
            {
                var resultBD = _contratoService.ObterTodos();
                var resultVM = Mapper.Map<IEnumerable<ComboContratoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObterContratoPorId(int id)
        {
            try
            {
                var resultBD = _contratoService.ObterContratoComServicosContratados(id);
                var resultVM = Mapper.Map<ContratoVM>(resultBD);
                var servicoComercial = _servicoContratadoService.ObterServicoComercialAtivoPorContrato(resultVM.Id);
                if (servicoComercial != null)
                {
                    resultVM.IdCelulaComercial = servicoComercial.IdCelula;
                }

                return Ok(new
                {
                    dados = resultVM,
                    notifications = "",
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("ObterContratos/{id}")]
        public IActionResult ObterContratosPorCliente([FromRoute] int id)
        {
            try
            {
                var resultBD = _contratoService.ObterContratosPorCliente(id);
                var resultVM = Mapper.Map<List<ContratoVM>>(resultBD.ToList());
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("PopularComboClientePorIdCelula/{id}")]
        public IActionResult PopularComboClientePorIdCelula([FromRoute] int id)
        {
            try
            {
                var resultBD = _contratoService.PopularComboClientePorCelula(id);
                var resultVM = Mapper.Map<List<MultiselectViewModel>>(resultBD.ToList());
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }  
}
