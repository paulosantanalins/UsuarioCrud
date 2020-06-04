using AutoMapper;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using GestaoServico.Domain.OperacaoMigradaRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using Utils.Base;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OperacaoMigradaController : Controller
    {
        private readonly IOperacaoMigradaService _operacaoMigradaService;
        private readonly IServicoEAcessoService _servicoEAcessoService;

        public OperacaoMigradaController(
            IOperacaoMigradaService operacaoMigradaService, IServicoEAcessoService servicoEAcessoService)
        {
            _operacaoMigradaService = operacaoMigradaService;
            _servicoEAcessoService = servicoEAcessoService;
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoViewModelBase<OperacaoMigradaDTO> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<OperacaoMigradaDTO>>(filtro);
            var result = _operacaoMigradaService.Filtrar(filtroDto);
            return Ok(result);
        }

        [HttpPost("Filtrar-Agrupamentos")]
        public IActionResult FiltrarAgrupamentos([FromBody] FiltroGenericoViewModelBase<OperacaoMigradaDTO> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDtoBase<AgrupamentoDTO>>(filtro);
            var result = _operacaoMigradaService.FiltrarAgrupamentos(filtroDto);
            return Ok(result);
        }

        [HttpGet("servicos/{idGrupoDelivery}")]
        public IActionResult BuscarServicosPorGrupoDelivery([FromRoute] int idGrupoDelivery)
        {
            var result = _operacaoMigradaService.BuscarServicosPorGrupoDelivery(idGrupoDelivery);
            return Ok(result);
        }

        [HttpPost("servicos-completos")]
        public IActionResult ObterServicosCompletos([FromBody] List<int> idsServicos)
        {
            var result = _servicoEAcessoService.ObterServicosCompletos(idsServicos);
            return Ok(result);
        }
    }
}
