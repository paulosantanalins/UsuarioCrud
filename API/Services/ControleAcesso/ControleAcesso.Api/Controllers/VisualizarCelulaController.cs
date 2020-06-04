using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utils;
using Utils.Base;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/VisualizacaoCelula")]
    public class VisualizarCelulaController : Controller
    {
        protected readonly IVisualizacaoCelulaService _visualizacaoCelulaService;
        private readonly MicroServicosUrls _microServicosUrls;

        public VisualizarCelulaController(
            IVisualizacaoCelulaService visualizacaoCelulaService,
            MicroServicosUrls microServicosUrls)
        {
            _visualizacaoCelulaService = visualizacaoCelulaService;
            _microServicosUrls = microServicosUrls;
        }

        [HttpPost]
        public IActionResult PersistirCelulasVinculadas([FromBody]VisualizacaoCelulaVM usuariosVisualizacaoPerfil)
        {
            try
            {
                List<VisualizacaoCelula> visualizacaoCelulaList = new List<VisualizacaoCelula>();
                MontarListaDeVisualizacao(usuariosVisualizacaoPerfil, visualizacaoCelulaList);
                _visualizacaoCelulaService.PersistirVisualizacaoCelula(visualizacaoCelulaList);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult AtualizarCelulasVinculadas([FromBody]VisualizacaoCelulaVM usuariosVisualizacaoPerfil)
        {
            try
            {
                List<VisualizacaoCelula> visualizacaoCelulaList = new List<VisualizacaoCelula>();
                 MontarListaDeVisualizacao(usuariosVisualizacaoPerfil, visualizacaoCelulaList);
                _visualizacaoCelulaService.AtualizarVisualizacaoCelula(visualizacaoCelulaList);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("remover-visualizacao-todas-celulas")]
        public IActionResult RemoverVisualizacaoDeTodasAsCelulas([FromBody] VisualizacaoCelulaVM visualizacaoCelulaVM)
        {
            try
            {
                _visualizacaoCelulaService.RemoverVisualizacaoTodasCelulas(visualizacaoCelulaVM.Logins.FirstOrDefault());
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        private void MontarListaDeVisualizacao(VisualizacaoCelulaVM usuariosVisualizacaoPerfil, List<VisualizacaoCelula> visualizacaoCelulaList)
        {
            foreach (var login in usuariosVisualizacaoPerfil.Logins)
            {
                foreach (var celula in usuariosVisualizacaoPerfil.Celulas)
                {
                    VisualizacaoCelula visualizacao = new VisualizacaoCelula();
                    visualizacao.LgUsuario = login.Split('|')[0].Trim();
                    visualizacao.IdCelulaUsuarioVinculado = Convert.ToInt32(login.Split(new string[] { "CEL" }, StringSplitOptions.None)[1].Trim());                    
                    visualizacao.IdCelula = celula.Id;                    
                    visualizacao.TodasAsCelulasSempre = usuariosVisualizacaoPerfil.TodasAsCelulasSempre;
                    visualizacao.TodasAsCelulasSempreMenosAPropria = usuariosVisualizacaoPerfil.TodasAsCelulasSempreMenosAPropria;
                    visualizacaoCelulaList.Add(visualizacao);
                }
            }
        }
        
        [HttpGet("buscar-usuarios-dados-visualizar/{login}")]
        public IActionResult ObterUsuarioComDadosVisualizar([FromRoute]string login, int idCelula)
        {
            try
            {
                var result = _visualizacaoCelulaService.ObterVisualizacaoCelularPorLogin(login, idCelula);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{login}")]
        public IActionResult BuscarVinculosUsuarioPorLogin([FromRoute] string login)
        {
            try
            {
                VisualizacaoCelulaVM vc = new VisualizacaoCelulaVM();
                var resultBD = _visualizacaoCelulaService.BuscarPorLogin(login);
                vc.TodasAsCelulasSempreMenosAPropria = resultBD.FirstOrDefault().TodasAsCelulasSempreMenosAPropria;
                vc.Logins.Add(resultBD.FirstOrDefault().LgUsuario);
                foreach (var visualizacao in resultBD)
                {
                    var celulaVM = Mapper.Map<CelulaVM>(visualizacao.Celula);

                    vc.Celulas.Add(celulaVM);
                }
                return Ok(new { dados = vc, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("celulas-visualizadas/{login}")]
        public IActionResult BuscarCelularVisualizadasPorLogin([FromRoute] string login, int celulaUsuario)
        {
            try
            {
                var result = _visualizacaoCelulaService.ObterVisualizacaoCelularPorLogin(login, celulaUsuario);
                var celulas = Mapper.Map<List<CelulaVM>>(result);

                return Ok(celulas.Select(x => x.Id).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("filtrar-usuarioad-celula-dropdown")]
        public IActionResult FiltrarUsuarioadCelulaDropdown(FiltroGenericoVM<UsuarioVisualizacaoCelulaDto> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDto<UsuarioVisualizacaoCelulaDto>>(filtroVM);
                var result = _visualizacaoCelulaService.FiltrarUsuariosCelulaDropdown(filtroDto);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }   
        
        [HttpPost("usuarios-ad-com-filtro-alguma-visualizacao")]
        public IActionResult ObterUsuariosAdComFiltro([FromBody]FiltroAdDtoSeguranca filtro)
        {
            try
            {
                var result = _visualizacaoCelulaService.ObterUsuariosAdComFiltroCelula(filtro);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("filtrarUsuarioAdPorCelulaComVisualizacao")]
        public IActionResult filtrarUsuarioAdPorCelulaComVisualizacao(FiltroGenericoVM<UsuarioVisualizacaoCelulaDto> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDto<UsuarioVisualizacaoCelulaDto>>(filtroVM);
                var usuariosComVisualizacaoCelula = _visualizacaoCelulaService.ObterUsuariosComVisualizacao(filtroDto);

                var usuariosADPorCelula = _visualizacaoCelulaService.BuscarUsuariosAdPorCelulas(filtroVM.ValorParaFiltrar);
                var result = usuariosComVisualizacaoCelula.Valores
                    .Where(x => usuariosADPorCelula
                    .Any(y => y.Login.ToUpper().Trim() == x.LgUsuario.ToUpper().Trim()));

                foreach (var item in result)
                {
                    var usuario = usuariosADPorCelula.FirstOrDefault(x => x.Login.Trim().ToUpper().Equals(item.LgUsuario.Trim().ToUpper()));
                    item.NomeCompleto = usuario != null ? usuario.NomeCompleto : "";                 
                    item.IdCelula = usuario != null ? Convert.ToInt32(usuario.Celula.Split(" ")[1]) : -1;
                }

                filtroDto.Valores = result.ToList();
                var resultFiltro = _visualizacaoCelulaService.FiltrarGrid(filtroDto);

                return Ok((new { dados = resultFiltro, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("filtrarUsuarioAdPorCelulaSemVisualizacao")]
        public IActionResult filtrarUsuarioAdPorCelulaSemVisualizacao(FiltroGenericoVM<UsuarioVisualizacaoCelulaDto> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDto<UsuarioVisualizacaoCelulaDto>>(filtroVM);
                var usuariosComVisualizacaoCelula = _visualizacaoCelulaService.ObterUsuariosComVisualizacao(filtroDto);

                var usuariosADPorCelula = _visualizacaoCelulaService.BuscarUsuariosAdPorCelulas(filtroVM.ValorParaFiltrar);

                var result = usuariosADPorCelula.Where(x => !usuariosComVisualizacaoCelula.Valores.Any(y => y.LgUsuario.ToUpper().Trim() == x.Login.ToUpper().Trim()));

                var visualizacaoDTO = Mapper.Map<List<UsuarioVisualizacaoCelulaDto>>(result);
                filtroDto.Valores = visualizacaoDTO.OrderBy(x => x.LgUsuario).ToList();

                return Ok((new { dados = filtroDto, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

    }
}


