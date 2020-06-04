using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DescontoController : Controller
    {
        private readonly IPrestadorService _prestadorService;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IDominioRepository _dominioRepository;
        private readonly IPluginRMService _pluginRMService;
        private readonly IHorasMesPrestadorService _horasMesPrestadorService;
        private readonly IDescontoPrestadorService _descontoPrestadorService;

        public DescontoController(
            IPrestadorService prestadorService,
            ICidadeRepository cidadeRepository,
            IDominioRepository dominioRepository,
            IPluginRMService pluginRMService,
            IHorasMesPrestadorService horasMesPrestadorService,
            IDescontoPrestadorService descontoPrestadorService)
        {
            _prestadorService = prestadorService;
            _cidadeRepository = cidadeRepository;
            _dominioRepository = dominioRepository;
            _pluginRMService = pluginRMService;
            _horasMesPrestadorService = horasMesPrestadorService;
            _descontoPrestadorService = descontoPrestadorService;
        }

        [HttpPost("filtrar-desconto-prestador")]
        public IActionResult FiltrarDescontoPrestador([FromBody] FiltroGenericoViewModelBase<DescontoPrestadorVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<DescontoPrestadorDto>>(filtro);
            var resultBD = _descontoPrestadorService.Filtrar(filtroDto);
            var resultVM = Mapper.Map<FiltroGenericoViewModelBase<DescontoPrestadorDto>>(resultBD);
            return Ok(resultVM);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            var resultBD = _descontoPrestadorService.BuscarPorId(id);
            var resultVM = Mapper.Map<EmpresaVM>(resultBD);
            return Ok(new { dados = resultVM, notifications = "", success = true });
        }

        [HttpPost("Persistir")]
        public IActionResult Persistir([FromBody] DescontoPrestadorVM descontoPrestadorVM)
        {
            if (descontoPrestadorVM.Id == 0)
            {
                var descontoPrestador = Mapper.Map<DescontoPrestador>(descontoPrestadorVM);
                _descontoPrestadorService.SalvarDescontoPrestador(descontoPrestador);
            }
            else
            {
                var descontoPrestador = Mapper.Map<DescontoPrestador>(descontoPrestadorVM);
                _descontoPrestadorService.AtualizarDescontoPrestadorr(descontoPrestador);
            }
            return Ok(true);
        }

        [HttpPost("Inativar")]
        public IActionResult Inativar([FromBody] int id)
        {
            _descontoPrestadorService.Inativar(id);
            return Ok(new { dados = "", notifications = "", success = true });
        }

        [HttpGet("obter-prestadores-por-celula/{id}/{idHorasMes}")]
        public IActionResult ObterPrestadoresPorCelula([FromRoute] int id, int idHorasMes)
        {
            var resultBD = _descontoPrestadorService.ObterPrestadoresPorCelula(id, idHorasMes);
            var resultVM = Mapper.Map<List<ComboDescontoPrestadorVM>>(resultBD).OrderByDescending(x => x.PodeReceberDesconto);
            return Ok(resultVM);
        }
    }
}
