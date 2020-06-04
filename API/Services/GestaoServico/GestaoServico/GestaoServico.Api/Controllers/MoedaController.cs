using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Domain.Core.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MoedaController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public MoedaController(NotificationHandler notificationHandler, IOptions<ConnectionStrings> connectionStrings)
        {
            _notificationHandler = notificationHandler;
            _connectionStrings = connectionStrings;
        }


        [HttpGet]
        public IActionResult Obtertodos()
        {
            try
            {
                var moedaEacessoService = new MoedaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = moedaEacessoService.ObterMoedas();
                var resultVM = Mapper.Map<IEnumerable<MoedaVM>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            try
            {
                var moedaEacessoService = new MoedaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = moedaEacessoService.ObterPorId(id);
                var resultVM = Mapper.Map<MoedaVM>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
