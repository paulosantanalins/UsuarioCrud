using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Usuario.Domain.BeneficioRoot.DTO;
using Usuario.Domain.BeneficioRoot.Entity;
using Usuario.Domain.BeneficioRoot.Service.Interfaces;
using Usuario.Domain.SharedRoot.Service.Interface;

namespace Usuario.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BeneficioController : Controller
    {
        private readonly IBeneficioStfcorpService _beneficioStfcorpService;
        private readonly IBaseService<BeneficioEAcesso> _baseBeneficioService;
        private readonly IBaseService<BeneficioParametroNatcorp> _baseParametroBeneficioNatcorp;
        private readonly IMapper _mapper;

        public BeneficioController(
            IBeneficioStfcorpService beneficioStfcorpService,
            IBaseService<BeneficioEAcesso> baseBeneficioService,
            IBaseService<BeneficioParametroNatcorp> baseParametroBeneficioNatcorp,
            IMapper mapper
          )
        {
            _beneficioStfcorpService = beneficioStfcorpService;
            _baseBeneficioService = baseBeneficioService;
            _baseParametroBeneficioNatcorp = baseParametroBeneficioNatcorp;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult BuscarBeneficio([FromRoute] int id)
        {
            var beneficio = _beneficioStfcorpService.BuscarPorId(id);
            var beneficioDto = _mapper.Map<BeneficioStfcorpDto>(beneficio);
            return Ok(beneficioDto);
        }


        [HttpGet("buscar-pagfornec/{idTipoBeneficio}/{idPropriedadeBeneficio}")]
        public IActionResult BuscarGrupoFaturamento([FromRoute] int idTipoBeneficio,[FromRoute] int idPropriedadeBeneficio)
        {
            var beneficio = _beneficioStfcorpService.BuscarGruposFaturamento(idTipoBeneficio, idPropriedadeBeneficio);

            return Ok(beneficio);
        }

        [HttpPost()]
        public IActionResult Persistir([FromBody] BeneficioStfcorpDto beneficioDto)
        {
            BeneficioStfcorp beneficio = _mapper.Map<BeneficioStfcorp>(beneficioDto);
            BeneficioEAcesso beneficioEacesso = _mapper.Map<BeneficioEAcesso>(beneficioDto);
            _beneficioStfcorpService.Persistir(beneficio, beneficioEacesso, beneficioDto);

            return Ok();
        }


        [HttpGet("buscar-usuarios")]
        public IActionResult BuscarTodosEventosPessoas()
        {
            var beneficio = _beneficioStfcorpService.BuscarTodosEventosETiposNatcorp();
            return Ok(beneficio);
        }


        [HttpGet("todos-eventos-e-tipos")]
        public IActionResult BuscarTodosEventos()
        {
            var beneficio = _beneficioStfcorpService.BuscarTodosEventosETiposNatcorp();
            return Ok(beneficio);
        }

        [HttpGet("buscar-beneficio-por-codigo/{codigo}")]
        public IActionResult BuscarBeneficioPorCodigo(string codigo)
        {
            var beneficio = _beneficioStfcorpService.BuscarBeneficioPorCodigo(codigo);
            return Ok(beneficio);
        }

        [HttpGet("todos-eventos-e-tipos-nao-associados")]
        public IActionResult BuscarTodosEventosNaoAssociados()
        {
            var beneficio = _beneficioStfcorpService.BuscarEventosAindaNaoAssociados();
            return Ok(beneficio);
        }

        [HttpGet("parametro-beneficio-natcorp/{id}")]
        public IActionResult BuscarBeneficioNatcorp([FromRoute] int Id)
        {
            var tipoNatCorp = _baseParametroBeneficioNatcorp.BuscarPorId(Id);
            //var tipoNatCorpDTO = Mapper.Map<ParametroBeneficioNarcorpDto>(tipoNatCorp);
            return Ok(tipoNatCorp);
        }

        [HttpPost("filtrar")]
        public IActionResult Filtrar([FromBody] FiltroBeneficioStfcorpDto<BeneficioStfcorpDto> filtro)
        {
            var result = _beneficioStfcorpService.Filtrar(filtro);
            return Ok(result);
        }

        [HttpGet("todos")]
        public IActionResult BuscarBeneficioNatcorp()
        {
            var todos = _beneficioStfcorpService.BuscarTodos();
            return Ok(todos);
        }

        [HttpGet("dados-contrato-beneficio")]
        public IActionResult ObterDadosDeBeneficioDoRM(string cnpjFornecedor)
        {
            var dados = _beneficioStfcorpService.ObterDadosDeBeneficioDoRM(cnpjFornecedor);
            return Ok(dados);
        }

        [HttpGet("tipos-natcorp/{idFamilia}")]
        public IActionResult BuscarDadosParaAssociacaoBeneficio(string idFamilia)
        {
            var dados = _beneficioStfcorpService.ObterTiposBeneficioNatcorp(idFamilia);
            return Ok(dados);
        }

        [HttpGet("eventos-saude-natcorp/{idFamilia}")]
        public IActionResult BuscarEventosEscopoSaudeNatcorp([FromRoute] string idFamilia)
        {
            var dados = _beneficioStfcorpService.ObterEventosBeneficioSaudeNatcorp(idFamilia);
            return Ok(dados);
        }


        [HttpPut("persistir-tipos-beneficio")]
        public IActionResult PersistirTiposBeneficio([FromBody] PersistirTiposBeneficioDto persistenciaDto)
        {
            _beneficioStfcorpService.PersistirTiposBeneficio(persistenciaDto);
            return Ok();
        }

        [HttpPut("remover-associacao-tipo/{idBeneficio}/{tipo}")]
        public IActionResult RemoverAssociacaoTipoBeneficio([FromRoute] int idBeneficio, [FromRoute] string tipo)
        {
            _beneficioStfcorpService.RemoverAssociacaoTipoBeneficio(idBeneficio, tipo);
            return Ok();
        }

        [HttpPut("remover-associasao-evento/{idBeneficio}/{eventoBase}")]
        public IActionResult RemoverAssociacaoEvento([FromRoute] int idBeneficio, [FromRoute] int eventoBase)
        {
            _beneficioStfcorpService.RemoverAssociacaoEvento(idBeneficio, eventoBase);
            return Ok();
        }

        [HttpGet("tipos-seguro-natcorp/{idBeneficio}")]
        public IActionResult BuscarTodosTiposSegurosNaoAssociados(int idBeneficio)
        {
            var dados = _beneficioStfcorpService.ObterTiposSeguroNatcorp(idBeneficio);
            return Ok(dados);
        }

        [HttpGet("tipos-seguro-natcorp-associados/{idBeneficio}")]
        public IActionResult BuscarTiposSegurosAssociados(int idBeneficio)
        {
            var dados = _beneficioStfcorpService.ObterTiposSeguroNatcorpAssociados(idBeneficio);
            return Ok(dados);
        }

        [HttpGet("seguro-sindicatos-natcorp/{idBeneficio}")]
        public IActionResult BuscarSegurosSindicatos(int idBeneficio)
        {
            var dados = _beneficioStfcorpService.ObterSegurosSindicatosNatcorp(idBeneficio);
            return Ok(dados);
        }

        [HttpGet("seguro-sindicatos-natcorp-associados/{idBeneficio}")]
        public IActionResult BuscarSegurosSindicatosAssociados(int idBeneficio)
        {
            var dados = _beneficioStfcorpService.ObterSeguroSindicatosNatcorpAssociados(idBeneficio);
            return Ok(dados);
        }

        [HttpGet("tipos-cartoes-nao-associados")]
        public IActionResult BuscaTiposNatcorpCartoesNaoAssociados(string familia)
        {
            var dados = _beneficioStfcorpService.BuscarTiposCartoesNatcorpNaoAssociados(familia);
            return Ok(dados);
        }

        [HttpGet("seguro-sindicatos-buscar-empresas-sindicatos/")]
        public IActionResult BuscarTodasEmpresasSindicatos()
        {
            var dados = _beneficioStfcorpService.ObterTodasEmpresasSindicatos();
            return Ok(dados);
        }

        [HttpGet("seguro-sindicatos-buscar-empresas/")]
        public IActionResult BuscarTodasEmpresas()
        {
            var dados = _beneficioStfcorpService.ObterTodasEmpresas();
            return Ok(dados);
        }
    }
}
