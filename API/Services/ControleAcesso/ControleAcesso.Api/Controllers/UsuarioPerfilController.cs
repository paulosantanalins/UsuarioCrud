using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Infra.CrossCutting.IoC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Utils.Base;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/UsuarioPerfil")]
    public class UsuarioPerfilController : Controller
    {
        protected readonly IUsuarioPerfilService _usuarioPerfilService;
        protected readonly IVisualizacaoCelulaService _visualizacaoCelulaService;

        public UsuarioPerfilController(
            IUsuarioPerfilService usuarioPerfilService,
            IVisualizacaoCelulaService visualizacaoCelulaService)
        {
            _usuarioPerfilService = usuarioPerfilService;
            _visualizacaoCelulaService = visualizacaoCelulaService;
        }

        [HttpGet("{login}")]
        public IActionResult ObterUsuarioComPerfil([FromRoute]string login)
        {
            var result = _usuarioPerfilService.ObterUsuarioPerfilComFuncionalidades(login);
            return Ok((new { dados = result, notifications = "", success = true }));
        }

        [HttpGet("filtrar")]
        public IActionResult ObterUsuarioAdPorLogin(FiltroGenericoVM<UsuarioPerfilDto> filtroVM)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<UsuarioPerfilDto>>(filtroVM);
            var result = _usuarioPerfilService.FiltrarUsuario(filtroDto);
            return Ok((new { dados = result, notifications = "", success = true }));
        }

        [HttpPost("filtrar")]
        public IActionResult ObterUsuarioAdPorLoginPost([FromBody] FiltroGenericoVM<UsuarioPerfilDto> filtroVM)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<UsuarioPerfilDto>>(filtroVM);
            var result = _usuarioPerfilService.FiltrarUsuario(filtroDto);
            return Ok((new { dados = result, notifications = "", success = true }));
        }

        private static MicroServicosUrls RecuperarMicroServicosUrls()
        {
            var microServicosUrls = Injector.ServiceProvider.GetService(typeof(MicroServicosUrls)) as MicroServicosUrls;
            return microServicosUrls;
        }        

        [HttpPost]
        public IActionResult PersistirVinculoUsuarioPerfil([FromBody]List<VinculoUsuarioPerfilVM> vinculoUsuarioPerfil)
        {
            try
            {
                var vinculosDto = Mapper.Map<List<VinculoUsuarioPerfilDto>>(vinculoUsuarioPerfil);
                _usuarioPerfilService.PersistirVinculos(vinculosDto);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult AtualizarVinculoUsuarioPerfil([FromBody]List<VinculoUsuarioPerfilVM> vinculoUsuarioPerfil)
        {
            try
            {
                var vinculosDto = Mapper.Map<List<VinculoUsuarioPerfilDto>>(vinculoUsuarioPerfil);
                _usuarioPerfilService.AtualizarVinculos(vinculosDto);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("ObterUsuarioComPerfisPorLogin/{login}")]
        public IActionResult ObterUsuarioComPerfisPorLogin([FromRoute]string login)
        {
            try
            {
                var result = _usuarioPerfilService.ObterUsuarioComPerfisPorLogin(login);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{login}/remover-vinculos")]
        public IActionResult RemoverVinculos(string login)
        {
            try
            {
                _usuarioPerfilService.RemoverVinculos(login);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-funcionalidades/{login}")]
        public IActionResult ObterFuncionalidadesComPerfil([FromRoute]string login)
        {
            try
            {
                var result = _usuarioPerfilService.ObterUsuarioPerfilComFuncionalidades(login);                
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("verifica-prestador-master/{login}")]
        public IActionResult VerificaPrestadorMaster([FromRoute]string login)
        {
            try
            {
                var result = _usuarioPerfilService.VerificaPrestadorMaster(login);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost("obter-email-por-funcionalidades/")]
        public IActionResult ObterEmailUsuarioPorFuncionalidade([FromBody]string[] funcionalidade)
        {
            try
            {
                var result = _usuarioPerfilService.ObterEmailPorFuncionalidade(funcionalidade);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("filtrarUsuarioAdPorCelula")]
        public IActionResult FiltrarUsuarioAdPorCelula(FiltroGenericoVM<UsuarioPerfilDto> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDto<UsuarioPerfilDto>>(filtroVM);
                var usuariosADPorCelula = _visualizacaoCelulaService.BuscarUsuariosAdPorCelulas(filtroVM.ValorParaFiltrar);
                filtroDto.ValorParaFiltrar = String.Join(",",usuariosADPorCelula.Select(x => x.Login));
                
                filtroDto = _usuarioPerfilService.ObterUsuariosComPerfil(filtroDto);

                return Ok((new { dados = filtroDto, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("obter-atualizacao-perfil/{login}")]
        public async Task<IActionResult> ObterAtualizacaoPerfil([FromRoute]string login)
        {
            var result = _usuarioPerfilService.ObterUsuarioPerfilComFuncionalidades(login);

            return await Task.Run(() => Ok(new
            {
                dados = result,
                notifications = "",
                success = true
            }));
        }
    }
}
