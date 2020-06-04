using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces;
using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces;
using RepasseEAcesso.Domain.SharedRoot.Service.Interface;
using System;
using System.Collections.Generic;
using Utils.Base;

namespace RepasseEAcesso.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepasseNivelUmController : ControllerBase
    {
        private readonly IPeriodoRepasseService _periodoRepasseService;
        private readonly IRepasseNivelUmService _repasseNivelUmService;
        private readonly IBaseService<RepasseNivelUm> _repasseNivelUmBaseService;
        private readonly IMapper _mapper;

        public RepasseNivelUmController(
            IPeriodoRepasseService periodoRepasseService,
            IRepasseNivelUmService repasseNivelUmService,
            IBaseService<RepasseNivelUm> repasseNivelUmBaseService,
            IMapper mapper)
        {
            _periodoRepasseService = periodoRepasseService;
            _repasseNivelUmService = repasseNivelUmService;
            _repasseNivelUmBaseService = repasseNivelUmBaseService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult BuscarTodos()
        {
            var resultBD = _repasseNivelUmService.BuscarTodos();
            var resultDTO = _mapper.Map<List<RepasseNivelUmDto>>(resultBD);
            return Ok(resultDTO);
        }


        [HttpPost("filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoDtoBase<AprovarRepasseDto> filtroDto)
        {
            try
            {
                var resultBD = _repasseNivelUmService.Filtrar(filtroDto);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("filtrar-repasses-nivel-um")]
        public IActionResult FiltrarRepasses([FromBody] FiltroRepasseNivelUmDto<AprovarRepasseDto> filtroDto)
        {
            try
            {
                var resultBD = _repasseNivelUmService.FiltrarRepassesNivelUm(filtroDto);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("filtrar-repasses-nivel-dois")]
        public IActionResult FiltrarRepassesNivelDois([FromBody] FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> filtroDto)
        {
            try
            {
                var resultBD = _repasseNivelUmService.FiltrarRepassesNivelDois(filtroDto);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] RepasseNivelUmDto repasseNivelUmDto)
        {
            try
            {
                var repasseStfCorp = _mapper.Map<RepasseNivelUm>(repasseNivelUmDto);
                var repasseEacesso = _mapper.Map<Repasse>(repasseNivelUmDto);
                _repasseNivelUmService.Persistir(repasseStfCorp, repasseEacesso);
                return Ok();
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
                var resultBD = _repasseNivelUmService.BuscarComIncludeId(id);
                var resultDTO = _mapper.Map<RepasseNivelUmDto>(resultBD);
                return Ok(new { dados = resultDTO, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("aprovar-repasse")]
        public IActionResult AprovarRepasse([FromBody] AprovarRepasseDto aprovarRepasseDto)
        {
            try
            {
                _repasseNivelUmService.AprovarRepasse(aprovarRepasseDto);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("aprovar-repasse-nivel-dois")]
        public IActionResult AprovarRepasseNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto)
        {
            try
            {
                _repasseNivelUmService.AprovarRepasseNivelDois(aprovarRepasseDto);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("negar-aprovar-repasse")]
        public IActionResult NegarAprovarPagamento([FromBody] AprovarRepasseDto aprovarRepasseDto)
        {
            try
            {
                _repasseNivelUmService.NegarRepasse(aprovarRepasseDto);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("negar-repasse-nivel-dois")]
        public IActionResult NegarRepasseNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto)
        {
            try
            {
                _repasseNivelUmService.NegarRepasseNivelDois(aprovarRepasseDto);
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
