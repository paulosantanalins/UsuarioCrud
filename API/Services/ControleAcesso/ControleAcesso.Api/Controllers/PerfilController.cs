using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Api.ViewModels.ControleAcesso;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.Core.Notifications;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PerfilController : Controller
    {
        protected readonly IPerfilService _perfilService;
        protected readonly NotificationHandler _notificationHandler;

        public PerfilController(IPerfilService perfilService,
                                NotificationHandler notificationHandler)
        {
            _perfilService = perfilService;
            _notificationHandler = notificationHandler;
        }


        [HttpPost]
        public IActionResult Persistir([FromBody]PerfilVM perfilVM)
        {
            try
            {
                var perfil = Mapper.Map<Perfil>(perfilVM);
                _perfilService.PersistirPerfil(perfil);
                return Ok(new
                {
                    dados = "",
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("Inativar/{id}")]
        public IActionResult InativarPerfil([FromRoute]int id)
        {
            try
            {
                _perfilService.InativarPerfil(id);
                return Ok(new
                {
                    dados = "",
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("VerificarExistenciaUsuariosComPerfil/{id}")]
        public IActionResult VerificarPossivelInativacao([FromRoute]int id)
        {
            try
            {
                var result = _perfilService.VerificarExistenciaUsuariosComPerfil(id);
                return Ok(new
                {
                    dados = result,
                    notifications = _notificationHandler.Mensagens.Any() ? _notificationHandler.Mensagens.Select(x => x._value) : new List<string>(),
                    success = !_notificationHandler.Mensagens.Any()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoVM<PerfilVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<PerfilDto>>(filtro);
            try
            {
                var resultBD = _perfilService.FiltrarPerfil(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<PerfilVM>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
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
                var resultBD = _perfilService.BuscarPorId(id);
                var resultVM = Mapper.Map<PerfilVM>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public IActionResult BuscarTodos()
        {
            try
            {
                var resultBD = _perfilService.BuscarTodosPerfis();
                var resultVM = Mapper.Map<List<PerfilVM>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-ativos")]
        public IActionResult BuscarTodosAtivos()
        {
            try
            {
                var resultBD = _perfilService.BuscarTodosPerfisAtivos();
                var resultVM = Mapper.Map<List<PerfilVM>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        
    }
}
