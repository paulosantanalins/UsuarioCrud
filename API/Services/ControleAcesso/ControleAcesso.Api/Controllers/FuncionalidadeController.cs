using AutoMapper;
using ControleAcesso.Api.ViewModels.ControleAcesso;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.Core.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Funcionalidade")]
    public class FuncionalidadeController : Controller
    {
        protected readonly NotificationHandler _notificationHandler;
        protected readonly IFuncionalidadeService _funcionalidadeService;
        private readonly IHttpContextAccessor _contextAccessor;

        public FuncionalidadeController(NotificationHandler notificationHandler,
                                        IFuncionalidadeService funcionalidadeService,
                                        IHttpContextAccessor contextAccessor)
        {
            _notificationHandler = notificationHandler;
            _funcionalidadeService = funcionalidadeService;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public IActionResult BuscarTodasFuncionalidades()
        {
            try
            {
                var resultBD = _funcionalidadeService.BuscarTodasFuncionalidades();             
                var resultVM = Mapper.Map<List<FuncionalidadeVM>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
