using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.StfAnalitcsDW.Model;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/ServicoContratados")]
    public class ServicoContratadoController : Controller
    {
        private readonly IServicoContratadoService _servicoContratadoService;
        private readonly IContratoService _contratoService;
        private readonly NotificationHandler _notificationHandler;
        private readonly Variables _variables;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IServicoEAcessoService _servicoEAcessoService;

        public ServicoContratadoController(
            IServicoContratadoService servicoContratadoService, 
            NotificationHandler notificationHandler,
            Variables variables,
            MicroServicosUrls microServicosUrls,
            IContratoService contratoService, IServicoEAcessoService servicoEAcessoService)
        {
            _servicoContratadoService = servicoContratadoService;
            _notificationHandler = notificationHandler;
            _variables = variables;
            _microServicosUrls = microServicosUrls;
            _contratoService = contratoService;
            _servicoEAcessoService = servicoEAcessoService;
        }

        [HttpGet]
        public IActionResult Filtrar([FromQuery]FiltroGenericoViewModel<ServicoContratadoVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ServicoContratadoDto>>(filtro);
            try
            {
                FiltroGenericoDto<ServicoContratadoDto> resultBD = _servicoContratadoService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoViewModel<ServicoContratadoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Persistir([FromBody]ServicoContratadoVM ServicoContratadoVM)
        {
            var servico = Mapper.Map<ServicoContratado>(ServicoContratadoVM);
            try
            {
                servico.EscopoServico = null;
                _servicoContratadoService.Persistir(servico, ServicoContratadoVM.IdCelula.Value, ServicoContratadoVM.VlMarkup);
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

        [HttpPost("servico-migrado")]
        public IActionResult PersistirServicoMigrado([FromBody]ServicoMigracaoDTO servicoMigracaoDTO)
        {
            _servicoContratadoService.PersistirServicoMigrado(servicoMigracaoDTO);
            return Ok();
        }
            

        [HttpPost("filtrar")]
        public IActionResult FiltrarPost([FromBody]FiltroGenericoViewModel<ServicoContratadoVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ServicoContratadoDto>>(filtro);
            try
            {
                FiltroGenericoDto<ServicoContratadoDto> resultBD = _servicoContratadoService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoViewModel<ServicoContratadoVM>>(resultBD);

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
                var resultBD = _servicoContratadoService.BuscarPorId(id);
                var resultVM = Mapper.Map<ServicoContratadoVM>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("eacesso/{idCelula}/{idCliente}")]
        public IActionResult GetServicoPorIdCelulaIdClienteEAcesso([FromRoute]int idCelula, [FromRoute]int idCliente)
        {
            try
            {
                var resultStfCorp = _servicoEAcessoService.ObterServicoAtivoPorIdCelulaIdClienteEAcesso(idCelula, idCliente);
                return Ok(resultStfCorp);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet]
        [Route("eacesso/todos/{idCelula}/{idCliente}")]
        public IActionResult GetTodosServicoPorIdCelulaIdClienteEAcesso([FromRoute]int idCelula, [FromRoute]int idCliente)
        {
            try
            {
                var resultStfCorp = _servicoEAcessoService.ObterTodosServicoPorIdCelulaIdClienteEAcesso(idCelula, idCliente);
                return Ok(resultStfCorp);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet("PreencherComboServicoContratado")]
        public IActionResult PreencherComboServicoContratado()
        {
            try
            {
                var resultBD = _servicoContratadoService.PreencherComboServicoContratado();
                var resultVM = Mapper.Map<List<MultiselectViewModel>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("PreencherComboServicoContratadoPorCelulaCliente/{idCelula}/{idCliente}")]
        public IActionResult PreencherComboServicoContratadoPorCelulaCliente(int idCelula, int idCliente)
        {
            try
            {
                var resultBD = _servicoContratadoService.PreencherComboServicoContratadoPorCelulaCliente(idCelula, idCliente);
                var resultVM = Mapper.Map<List<MultiselectViewModel>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idCliente}/obter-pacotes-por-cliente")]
        public IActionResult ObterServicoContratadoPorCliente(int idCliente)
        {
            try
            {
                var resultBD = _servicoContratadoService.ObterServicoContratadoPorCliente(idCliente);
                var resultVM = Mapper.Map<List<MultiselectViewModel>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idCelula}/obter-pacotes-por-celula-relatorio-rentabilidade")]
        public IActionResult ObterServicoContratadoPorCelulaRelatorioRentabilidade(int idCelula)
        {
            try
            {
                var resultBD = _servicoContratadoService.ObterServicoContratadoPorCelulaRelatorioRentabilidade(idCelula);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idCelula}/{idCliente}/obter-pacotes-por-celula-cliente-relatorio-rentabilidade")]
        public IActionResult ObterServicoContratadoPorCelulaCliente(int idCelula, int idCliente)
        {
            try
            {
                var resultBD = _servicoContratadoService.ObterServicoContratadoPorClienteRelatorioRentabilidade(idCelula, idCliente);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-pacotes-por-celulas-relatorio-diretoria")]
        public IActionResult ObterServicoContratadoPorCelulaRelatorioRentabilidade(List<int> idsCelula)
        {
            try
            {
                var resultBD = _servicoContratadoService.ObterServicoContratadoPorCelulaRelatorioDiretoria(idsCelula);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("persistir-servico-eacesso")]
        public IActionResult PersistirServicoEacesso([FromBody]ViewServicoModel viewServico)
        {
            var servico = Mapper.Map<ServicoContratado>(viewServico);
            try
            {
                servico.EscopoServico = null;

                servico.DeParaServicos.Add(new DeParaServico
                {
                    DescStatus = "MA",
                    DescTipoServico = viewServico.SiglaTipoServico,
                    NmServicoEacesso = viewServico.NomeServico,
                    DescEscopo = viewServico.DescEscopo,
                    DescSubTipoServico = viewServico.SubTipo,
                    IdServicoEacesso = viewServico.IdServico,
                    DataAlteracao = DateTime.Now,
                    Usuario = "Eacesso"
                });
                viewServico.IdCliente = ObterClienteEacessoPorId(viewServico.IdCliente);
                servico.IdContrato = _contratoService.VerificarExistenciaContratoEacesso(viewServico.IdContrato, viewServico.IdCliente);

                var idServico = _servicoContratadoService.Persistir(servico, servico.IdCelula, viewServico.Markup);
                return Ok(idServico);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("migrar-servico-eacesso")]
        public IActionResult MigrarServicoEacesso([FromBody]ServicoContratadoVM ServicoContratadoVM)
        {
            var servico = Mapper.Map<ServicoContratado>(ServicoContratadoVM);
            try
            {
                servico.EscopoServico = null;
                _servicoContratadoService.MigrarServicoEacesso(servico);
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

        [HttpGet("filtrar-accordions")]
        public IActionResult FiltrarAcordions([FromQuery]FiltroGenericoViewModel<ServicoContratadoVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ServicoContratado>>(filtro);
            try
            {
                var resultBD = _servicoContratadoService.FiltrarAccordions(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoViewModel<ServicoContratadoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("filtrar-servico-por-nome/{nome}")]
        public IActionResult FiltrarServicoPorNome([FromQuery] string nome)
        {
            var servico = _servicoEAcessoService.BuscarIdServicosPorNomeServico(nome);

            return Ok(servico);
        }

        [HttpGet("obter-servicos-por-ids/{ids}")]
        public IActionResult ObterServicosPorIds(string ids)
        {
            var servicos = _servicoEAcessoService.ObterServicosPorIdsEacesso(ids);

            return Ok(servicos);
        }

        [HttpGet("obter-servico-por-id/{id}")]
        public IActionResult ObterServicoPorId(int id)
        {
            var servico = _servicoEAcessoService.ObterServicoPorId(id);

            return Ok(servico);
        }

        private int ObterClienteEacessoPorId(int idCliente)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);

            client.BaseAddress = new Uri(_microServicosUrls.UrlApiCliente);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/cliente/" + idCliente + "/obter-cliente-migrado/").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Int32.Parse(responseString);
        }
    }
}
