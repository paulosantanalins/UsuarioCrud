using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CelulaController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly ICelulaService _celulaService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IVariablesToken _variables;

        public CelulaController(
            IVariablesToken variables,
            NotificationHandler notificationHandler,
            ICelulaService celulaService,
            IOptions<ConnectionStrings> connectionStrings,
            MicroServicosUrls microServicosUrls)
        {
            _notificationHandler = notificationHandler;
            _celulaService = celulaService;
            _variables = variables;
            _connectionStrings = connectionStrings;
            _microServicosUrls = microServicosUrls;
        }


        [HttpGet]
        public IActionResult Obtertodos()
        {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterCelulas();
                var resultVM = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-todas-celulas")]
        public IActionResult ObterTodasCelulas()
        {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterTodasCelulas();
                var resultVM = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("quantidade")]
        public IActionResult ObterQuantidade() {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterCelulas();

                return Ok(resultBD.Count);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-diretorias")]
        public IActionResult ObterDiretorias()
        {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterDiretorias();
                var resultVM = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-diretorias-com-visualizacao")]
        public IActionResult ObterDiretoriasComVisualizacao()
        {
            try
            {
                var usuario = _variables.UserName;
                List<CelulaVM> celulasComPermissao = BuscarCelulasVisualizadasPorUsuario(usuario);
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterDiretorias();
                var celulasDiretoria = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);
                var resultVM = celulasComPermissao.Where(x => celulasDiretoria.Any(y => y.Id == x.Id));
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("com-visualizacao")]
        public IActionResult ObterCelulasComVisualizacao()
        {
            try
            {
                var usuario = _variables.UserName;
                List<CelulaVM> celulasComPermissao = BuscarCelulasVisualizadasPorUsuario(usuario);

                return Ok(new { dados = celulasComPermissao, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }     

        [HttpGet("{idCelula}/obter-subordinadas")]
        public IActionResult ObterSubordinadas(int idCelula)
        {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterSubordinadas(idCelula);
                var resultVM = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-comerciais")]
        public IActionResult ObterComerciais()
        {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterCelulasComerciais();
                var resultVM = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-estrutura-hierarquica")]
        public IActionResult ObterEstruturaHierarquica()
        {
            try
            {
                var celulaEacessoService = new CelulaEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = celulaEacessoService.ObterEstruturaCelulas();
                var resultVM = Mapper.Map<IEnumerable<CelulaVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private List<CelulaVM> BuscarCelulasVisualizadasPorUsuario(string usuario)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/VisualizacaoCelula/buscar-usuarios-dados-visualizar/" + usuario).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;
            var retorno = JsonConvert.DeserializeObject<RetornoCelulasVM>(responseString);
            var celulasVM = Mapper.Map<List<CelulaVM>>(retorno.Dados);
            return celulasVM;
        }
    }
}
