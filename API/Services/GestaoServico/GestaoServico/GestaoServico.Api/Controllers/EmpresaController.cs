using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoFilialRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Api.Controllers
{
     [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmpresaController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IEmpresaService _empresaService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public EmpresaController(NotificationHandler notificationHandler, IEmpresaService empresaService, IOptions<ConnectionStrings> connectionStrings)
        {
            _notificationHandler = notificationHandler;
            _empresaService = empresaService;
            _connectionStrings = connectionStrings;
        }


        [HttpGet("{status}")]
        public IActionResult Obtertodos(string status)
        {
            try
            {
                var empresaEacessoService = new EmpresaEacessoService(_connectionStrings.Value.RMConnection);
                var resultBD = empresaEacessoService.ObterEmpresas(status);
                var resultVM = Mapper.Map<IEnumerable<EmpresaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
