
using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CelulaController : Controller
    {
        private readonly IVisualizacaoCelulaService _visualizacaoCelulaService;
        private readonly ICelulaService _celulaService;
        private readonly IVariablesToken _variables;

        public CelulaController(
            ICelulaService celulaService,
            IVisualizacaoCelulaService visualizacaoCelulaService, IVariablesToken variables)
        {
            _visualizacaoCelulaService = visualizacaoCelulaService;
            _variables = variables;
            _celulaService = celulaService;
        }

        [HttpGet]
        public IActionResult ObterTodos()
        {

            try
            {
                var result = _celulaService.ObterTodas();
                var celulas = Mapper.Map<ICollection<CelulaVM>>(result);
                return Ok(celulas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("celulas-que-pessoa-e-responsavel/{idPessoa}")]
        public IActionResult ObterCelulasQuePessoaEhResponsavel(int idPessoa)
        {
            try
            {
                var result = _celulaService.BuscarCelulasQuePessoaEhResponsavel(idPessoa);
                var celulas = Mapper.Map<ICollection<CelulaVM>>(result);
                return Ok(celulas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("eacesso")]
        public IActionResult ObterTodosEacesso()
        {
            try
            {
                var result = _celulaService.ObterTodasEacesso();
                var celulas = Mapper.Map<ICollection<CelulaVM>>(result);
                return Ok(celulas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("com-permissao-visualizacao")]
        public IActionResult ObterComPermissaoVisualizacao()
       {
            try
            {
                var result = _visualizacaoCelulaService.ObterVisualizacaoCelularPorLogin(_variables.UsuarioToken);
                var celulas = Mapper.Map<ICollection<CelulaVM>>(result);
                return Ok(celulas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoVM<CelulaDto> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<CelulaDto>>(filtro);
            try
            {
                var resultBD = _celulaService.FiltrarCelula(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<CelulaDto>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("buscar-por-id")]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                var resultBD = _celulaService.BuscarPorId(id);
                var celulaVM = Mapper.Map<CelulaDto>(resultBD);
                string cpf = string.Empty;
                if (resultBD.IdPessoaResponsavel != null)
                {
                    cpf = _celulaService.ObterPessoaApiCadastro(resultBD.IdPessoaResponsavel).Cpf;
                }

                if (!string.IsNullOrEmpty(cpf))
                {
                    var (idCelula, login) = _celulaService.ObterIdCelulaResponsavel(cpf);
                    celulaVM.IdCelulaResponsavel = idCelula;
                    celulaVM.LoginResponsavel = login;
                }

                return Ok(celulaVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-ultima-cadastrada")]
        public IActionResult BuscarUltimaCelulaCadastrada()
        {
            try
            {
                var resultBD = _celulaService.ObterUltimaCadastrada();
                var celulaDto = Mapper.Map<CelulaDto>(resultBD);
                return Ok(celulaDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        [HttpGet("email-gerente-servico/{idCelula}")]
        public IActionResult OterEmailGerenteServico(int idCelula)
        {
            try
            {
                var emailGS = _celulaService.ObterEmailGerenteServico(idCelula);
                return Ok(emailGS);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-tipo-celula")]
        public IActionResult ObterTodosTipoCelula()
        {

            try
            {
                var result = _celulaService.ObterTodosTiposCelula();
                var tipoCelula = Mapper.Map<ICollection<TipoCelulaVM>>(result);
                return Ok(tipoCelula);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }

        [HttpGet("obter-tipo-celula-e-contabil")]
        public IActionResult ObterTodosTipoCelulaeTipoContabil()
        {

            try
            {
                var result = _celulaService.ObterTodosTiposCelulatipoContabil();
                var tipoCelula = Mapper.Map<ICollection<VinculoTipoCelulaTipoContabilVM>>(result);
                return Ok(tipoCelula);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }

        [HttpPost("buscar-servicos-pendentes-por-idcelula")]
        public async Task<IActionResult> BuscarServicosPendentesPorIdCelula([FromBody]FiltroGenericoVM<ServicoCelulaDTO> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ServicoCelulaDTO>>(filtro);

            try
            {
                var servicos = _celulaService.BuscarServicosPendente(filtroDto);
                return Ok(servicos);
            }catch(Exception e)
            {
                return BadRequest(e);

            }

        }


        [HttpGet("buscar-logs-celula")]
        public async Task<IActionResult> BuscarLogsDeAlteracaoDeCampo(int idCelula)
        {
            try
            {
                List<LogCampoCelulaDto> logs = await _celulaService.ObterLogsDeAlteracaoDeCampoDeCelulaPorCelula(idCelula);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("inativar/{id}")]
        public IActionResult Inativar(int id)
        {
            try
            {
                _celulaService.Inativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPut("reativar/{id}")]
        public IActionResult Reativar(int id)
        {
            try
            {
                _celulaService.Reativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Persistir")]
        public IActionResult Persistir([FromBody] CelulaDto celulaDto)
        {
            try
            {
                var celula = Mapper.Map<Celula>(celulaDto);
                _celulaService.Persistir(celula, celulaDto.LoginResponsavel);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Valida-celula-existe")]
        public IActionResult ValidaCelulaExiste(int id)
        {
            try
            {
                var existeCelula = _celulaService.ValidarExisteCelula(id);
                return Ok(existeCelula);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("validar-servicos-pendentes-celula/{idCelula}")]
        public IActionResult ValidarServicosPendentesCelula([FromRoute]int idCelula)
        {
            var result = _celulaService.ExisteServicoPendente(idCelula);

            return Ok(new { result });
        }
    }
}
