using AutoMapper;
using ControleAcesso.Domain.MonitoramentoRoot.DTO;
using ControleAcesso.Domain.MonitoramentoRoot.Service.Interfaces;
using ControleAcesso.Domain.SharedRoot;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MonitoramentoController : Controller
    {
        private readonly IMonitoramentoBackService _monitoramentoBackService;


        public MonitoramentoController(
            IMonitoramentoBackService monitoramentoBackService)
        {
            _monitoramentoBackService = monitoramentoBackService;
        }

        [HttpPost("filtrar-backend")]
        public IActionResult FiltrarBackend([FromBody] FiltroGenericoDto<MonitoramentoBackDto> filtro)
        {
            var logs = _monitoramentoBackService.FiltrarBackend(filtro);

            return Ok(logs);
        }

        [HttpGet("popular-combo-origem")]
        public IActionResult PopularComboOrigem()
        {
            var valoresEntity = _monitoramentoBackService.PopularComboOrigem();
            var valoresCombo = Mapper.Map<List<ComboDefaultDto>>(valoresEntity).GroupBy(x => x.Descricao).Select(x => 
                new ComboDefaultDto { Id = 1, Descricao = x.Key }
            );

            return Ok(valoresCombo);
        }
    }
}
