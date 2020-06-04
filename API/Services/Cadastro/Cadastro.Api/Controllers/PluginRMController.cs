using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace Cadastro.Api.Controllers
{

    [Produces("application/json")]
    [Route("api/Plugin")]
    public class PluginRMController : Controller
    {
        protected readonly IPluginRMService _pluginRMService;
        private readonly IPrestadorService _prestadorService;
        private readonly IHorasMesPrestadorService _horasMesPrestadorService;
        private readonly IVariablesToken _variables;


        public PluginRMController(
            IPluginRMService pluginRMService,
            IHorasMesPrestadorService horasMesPrestadorService,
            IPrestadorService prestadorService, IVariablesToken variables)
        {
            _horasMesPrestadorService = horasMesPrestadorService;
            _pluginRMService = pluginRMService;
            _prestadorService = prestadorService;
            _variables = variables;
        }

        [HttpGet("{id}")]
        public IActionResult EnviarParaRm([FromRoute] int id)
        {
            _pluginRMService.SolicitarPagamentoRM(id);
            return Ok();
        }

        [HttpGet("atualizar-status-pagamentos")]
        public IActionResult AtualizarStatusPagamentos()
        {
            _variables.UserName = "STFCORP";
            _pluginRMService.AtualizarSituacaoApartirDoRm();
            return Ok();
        }

        [HttpGet("log-erro/{id}")]
        public IActionResult ObterLogErroRm([FromRoute]int id)
        {
            var LogErros = _pluginRMService.ObterLogErroRm(id);
            return Ok(LogErros);
        }

    }
}
