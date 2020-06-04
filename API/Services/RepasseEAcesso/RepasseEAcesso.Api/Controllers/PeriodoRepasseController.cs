using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces;
using RepasseEAcesso.Domain.SharedRoot.DTO;
using RepasseEAcesso.Domain.SharedRoot.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;
using Utils.Connections;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RepasseEAcesso.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodoRepasseController : Controller
    {

        private readonly IPeriodoRepasseService _periodoRepasseService;
        private readonly IBaseService<PeriodoRepasse> _basePeriodoRepasseService;
        private readonly IMapper _mapper;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly Variables _variables;

        public PeriodoRepasseController(IPeriodoRepasseService periodoRepasseService,
            IBaseService<PeriodoRepasse> basePeriodoRepasseService,
            IMapper mapper,
            IOptions<ConnectionStrings> connectionStrings,
            Variables variables)
        {
            _periodoRepasseService = periodoRepasseService;
            _connectionStrings = connectionStrings;
            _basePeriodoRepasseService = basePeriodoRepasseService;
            _mapper = mapper;
            _variables = variables;
        }

        [HttpGet]
        public IActionResult BuscarTodos()
        {
            try
            {
                var periodoRepasse = _basePeriodoRepasseService.BuscarTodos().OrderByDescending(x => x.DtAprovacaoFim);
                var periodoRepasseDto = _mapper.Map<IEnumerable<PeriodoRepasseDto>>(periodoRepasse);

                return Ok(periodoRepasseDto);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }



        [HttpPost("filtrar")]
        public IActionResult FiltrarPeriodoRepasse(FiltroGenericoDtoBase<PeriodoRepasseDto> filtroDto)
        {
            try
            {
                var result = _periodoRepasseService.FiltrarPeriodo(filtroDto);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        [HttpGet("buscar-por-id/{id}")]
        public IActionResult ObterPeriodoRepassePorId(int id)
        {
            try
            {
                var periodoRepasse = _basePeriodoRepasseService.BuscarPorId(id);
                var periodoRepasseVM = _mapper.Map<PeriodoRepasseDto>(periodoRepasse);

                return Ok(periodoRepasseVM);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("popular-ano-mes-referencia")]
        public IActionResult ObterPeriodoRepassePorId()
        {
            try
            {
                PeriodoRepasse periodoRepasse = _basePeriodoRepasseService.BuscarTodos().OrderBy(x => x.DtLancamento).LastOrDefault();
                var periodoRepasseVM = _mapper.Map<PeriodoRepasseDto>(periodoRepasse);
                return Ok(periodoRepasseVM);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("persistir")]
        public IActionResult Persistir(PeriodoRepasseDto periodoRepasseDto)
        {
            try
            {
                var periodoRepasse = _mapper.Map<PeriodoRepasse>(periodoRepasseDto);
                var periodoLancamento = new DateTime(periodoRepasseDto.AnoLancamento, periodoRepasseDto.MesLancamento, 1);
                periodoRepasse.DtLancamento = periodoLancamento;
                _periodoRepasseService.Persistir(periodoRepasse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("periodos")]
        public IActionResult BuscarPeriodos()
        {
            try
            {
                var resultBD = _basePeriodoRepasseService.BuscarTodos().ToList();
                resultBD = resultBD.OrderByDescending(x => x.DtLancamento).ToList();
                var resultDto = _mapper.Map<IEnumerable<ComboDefaultDto>>(resultBD);

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("obter-ultimo-periodo-cadastrado")]
        public IActionResult ObterUltimoPeriodoCadastrado()
        {
            try
            {
                var resultBD = _periodoRepasseService.BuscarPeriodoVigente();
                var resultDTO = _mapper.Map<PeriodoRepasseDto>(resultBD);
                return Ok(resultDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("obter-data-dias-uteis/{data}/{tipo}")]
        public IActionResult ObterDataDiasUteis(string data, string tipo)
        {
            try
            {
                DateTime dataFimAnalise = new DateTime();
                DateTime dataFimAprova = new DateTime();
                DateTime dataFimLancamento = new DateTime();

                DateTime dataFim = Convert.ToDateTime(data);
                List<DateTime> DataFimAnaliseEAprovacao = new List<DateTime>();
                if (tipo == "lancamento")
                {
                    dataFimLancamento = DateTime.Parse(data);
                    dataFimAnalise = _periodoRepasseService.ObterDataDiasUteis(dataFim, 2);
                    dataFimAprova = _periodoRepasseService.ObterDataDiasUteis(dataFim, 4);

                }
                if (tipo == "analise")
                {
                    dataFimLancamento = _periodoRepasseService.ObterDataDiasUteis(dataFim, -2);
                    dataFimAnalise = DateTime.Parse(data);
                    dataFimAprova = _periodoRepasseService.ObterDataDiasUteis(dataFim, 2);

                }
                if (tipo == "aprovacao")
                {
                    dataFimLancamento = _periodoRepasseService.ObterDataDiasUteis(dataFim, -4);
                    dataFimAnalise = _periodoRepasseService.ObterDataDiasUteis(dataFim, -2);
                    dataFimAprova = DateTime.Parse(data);
                }
                DataFimAnaliseEAprovacao.Add(dataFimLancamento);
                DataFimAnaliseEAprovacao.Add(dataFimAnalise);
                DataFimAnaliseEAprovacao.Add(dataFimAprova);

                return Ok(DataFimAnaliseEAprovacao);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ambiente")]
        public IActionResult ObterAmbiente()
        {
            return Ok(Variables.EnvironmentName.ToUpper().ToString());
        }
    }
}



