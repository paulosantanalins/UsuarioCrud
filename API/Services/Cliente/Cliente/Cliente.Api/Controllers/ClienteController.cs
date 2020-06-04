using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Dto;
using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Domain.ClienteRoot.Service.Interfaces;
using Cliente.Domain.Core.Notifications;
using Logger.Model;
using Logger.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;
using Utils.Extensions;
using Utils.Salesforce.Models;

namespace Cliente.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Cliente")]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogGenericoRepository _logGenericoRepository;
        private readonly NotificationHandler _notificationHandler;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        protected readonly IGrupoClienteService _grupoClienteService;
        protected readonly IEnderecoService _enderecoService;
        protected readonly IPaisRepository _paisRepository;

        public ClienteController(IClienteService clienteService,
                                 IClienteRepository clienteRepository,
                                 ILogGenericoRepository logGenericoRepository,
                                 NotificationHandler notificationHandler,
                                 IOptions<ConnectionStrings> connectionStrings,
                                 IGrupoClienteService grupoClienteService,
                                 IEnderecoService enderecoService,
                                 IPaisRepository paisRepository)
        {
            _clienteService = clienteService;
            _clienteRepository = clienteRepository;
            _logGenericoRepository = logGenericoRepository;
            _notificationHandler = notificationHandler;
            _connectionStrings = connectionStrings;
            _grupoClienteService = grupoClienteService;
            _enderecoService = enderecoService;
            _paisRepository = paisRepository;
        }

        [HttpGet]
        [Route("obterClientePorCNPJ/{cnpj}")]
        public async Task<IActionResult> GetClientePorCNPJ([FromRoute]string cnpj)
        {
            return Ok();
        }

        [HttpGet]
        [Route("ObterClientesPorCelula/{idCelula}/{isInativo}")]
        public IActionResult ObterClientesPorCelula([FromRoute]int? idCelula, [FromRoute]bool isInativo)
        {
            try
            {
                var clientes = _clienteService.ObterClientesEacessoPorIdCelula(idCelula, isInativo);
                return Ok(clientes);
            }
            catch (Exception)
            {
                return BadRequest();                
            }
          
        }

        [HttpGet]
        [Route("ObterClientesPorCelula/{idCelula}/{isInativo}/{login}")]
        public IActionResult ObterClientesPorCelulaEVisualizacaoServico([FromRoute]int? idCelula, [FromRoute]bool isInativo,[FromRoute]string login)
        {
            try
            {
                var clientes = _clienteService.ObterClientesEacessoPorIdCelulaEvisualizacaoServico(idCelula, isInativo, login);
                return Ok(clientes);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Route("obterClientePorIdSalesforce/{idSalesforce}")]
        public async Task<IActionResult> GetClientePorIdSalesforce([FromRoute]string idSalesforce)
        {
            try
            {
                var resultStfCorp = _clienteService.ObterClientePorIdSalesForce(idSalesforce);
                return Ok(resultStfCorp);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet]
        [Route("eacesso/{idCelula}")]
        public async Task<IActionResult> GetClientePorIdCelulaEAcesso([FromRoute]int idCelula)
        {
            try
            {
                var resultStfCorp = _clienteService.ObterClienteAtivoPorIdCelulaEAcesso(idCelula);
                return Ok(resultStfCorp);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet]
        [Route("eacesso/todos/{idCelula}")]
        public async Task<IActionResult> GetTodosClientePorIdCelulaEAcesso([FromRoute]int idCelula)
        {
            try
            {
                var resultStfCorp = _clienteService.ObterTodosClientePorIdCelulaEAcesso(idCelula);
                return Ok(resultStfCorp);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpPost]
        [Route("TratarClienteSalesforce")]
        public async Task<IActionResult> PersistirClienteSalesforce([FromBody]AccountSalesObject salesObject)
        {
            try
            {
                var pendente = false;
                salesObject.IdStfCorp = _clienteService.ObterClientePorIdSalesForce(salesObject.Id).HasValue ? _clienteService.ObterClientePorIdSalesForce(salesObject.Id).Value : 0;
                if (salesObject.IdStfCorp == 0)
                {
                    if (salesObject.Type == "Mãe")
                    {
                        salesObject.IdGrupoCliente = _grupoClienteService.ObterIdGrupoClientePorIdClienteMae(salesObject.Id);
                        if (salesObject.IdGrupoCliente == 0)
                        {
                            salesObject.IdGrupoCliente = _grupoClienteService.PersistirGrupoClienteSalesForce(salesObject.Id, salesObject.Name);
                            await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "Persistencia do cliente mãe: " + salesObject.Name + " concluida", DescExcecao = "" });
                        }
                    }
                    else
                    {
                        salesObject.IdGrupoCliente = _grupoClienteService.ObterIdGrupoClientePorIdClienteMae(salesObject.ParentId);
                        if (salesObject.IdGrupoCliente == 0)
                        {
                            salesObject.IdGrupoCliente = null;
                        }
                    }

                    var clienteEacessoService = new ClienteEacessoService(_connectionStrings.Value.EacessoConnection);
                    var clienteEacesso = clienteEacessoService.ObterClienteEacessoPorIdSalesForce(salesObject.Id);
                    ClienteET clienteStfCorp = null;
                    if (clienteEacesso != null)
                    {
                        salesObject = Mapper.Map(clienteEacesso, salesObject);
                    }

                    if (salesObject.CNPJ__c != null && salesObject.CNPJ__c.Trim() != "")
                    {
                        salesObject.CNPJ__c = _clienteService.ApenasNumeros(salesObject.CNPJ__c);

                    }
                    else
                    {
                        pendente = true;
                        await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui cnpj", DescExcecao = "" });
                        salesObject.CNPJ__c = _clienteService.ApenasNumeros("NWM9709244W4");
                    }

                    clienteStfCorp = Mapper.Map<ClienteET>(salesObject);
                    if (clienteStfCorp.Id == 0)
                    {
                        clienteStfCorp.Id = _clienteService.VerificarIdCliente();
                    }

                    var endereco = Mapper.Map<Endereco>(salesObject);

                    if (clienteStfCorp.NrTelefone != null)
                    {
                        clienteStfCorp.NrTelefone = clienteStfCorp.NrTelefone.Replace(" ", "");
                        if (clienteStfCorp.NrTelefone.Contains('/'))
                        {
                            var numeros = clienteStfCorp.NrTelefone.Split("/");
                            clienteStfCorp.NrTelefone = numeros[0];
                            clienteStfCorp.NrTelefone2 = numeros[0].Substring(0, 2) + numeros[1];
                        }
                    }
                    if (clienteStfCorp.NrTelefone2 != null)
                    {
                        clienteStfCorp.NrTelefone = clienteStfCorp.NrTelefone2.Replace(" ", "");
                    }
                    if (clienteStfCorp.NmRazaoSocial == null || !clienteStfCorp.NmRazaoSocial.Any())
                    {
                        clienteStfCorp.NmRazaoSocial = "";
                        pendente = true;
                        await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui uma razão social", DescExcecao = "" });
                    }
                    if (clienteStfCorp.NmFantasia == null || !clienteStfCorp.NmFantasia.Any())
                    {
                        clienteStfCorp.NmFantasia = "";
                        pendente = true;
                        await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui um nome fantasia", DescExcecao = "" });
                    }
                    endereco.IdCidade = _enderecoService.VerificarEndereco(salesObject.CNPJ_Cidade__c);

                    clienteStfCorp.Enderecos.Add(endereco);
                    if (endereco.IdCidade == null)
                    {
                        pendente = true;
                        await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui uma cidade valida", DescExcecao = "" });
                    }

                    if (!_clienteService.ValidarCNPJ(clienteStfCorp.NrCnpj, "BRASIL").Result)
                    {
                        pendente = true;
                        await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui um cnpj valido", DescExcecao = "" });
                    }

                    if (pendente)
                    {
                        clienteStfCorp.FlStatus = "P";
                    }
                    else
                    {
                        clienteStfCorp.FlStatus = "A";
                    }
                    salesObject.IdStfCorp = _clienteService.PersistirCliente(clienteStfCorp);
                }
                else
                {
                    await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " encontrado no sistema", DescExcecao = "" });
                }

                return Ok(salesObject.IdStfCorp);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Filtrar")]
        public IActionResult Filtrar([FromBody]FiltroGenericoViewModel<ClienteVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ClienteET>>(filtro);
            try
            {
                var resultBD = _clienteService.Filtrar(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<ClienteVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("obter-clientes-eacesso")]
        public IActionResult ObtertodosEacesso([FromBody]FiltroGenericoViewModel<ClienteVM> filtro)
        {
            var filtroDto = Mapper.Map<FiltroGenericoDto<ClienteEacessoDto>>(filtro);
            try
            {
                var resultEacesso = _clienteService.ObterClientesEacesso(filtroDto);
                var resultVM = Mapper.Map<FiltroGenericoDto<ClienteVM>>(resultEacesso);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-cliente-eacesso")]
        public IActionResult ObtertodosEacesso(int idCliente)
        {   
            try
            {
                var resultEacesso = _clienteService.ObterClienteEacesso(idCliente);
                var resultVM = Mapper.Map<ClienteVM>(resultEacesso);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-local-trabalho-eacesso")]
        public IActionResult ObterLocalTrabalhoEacesso(int idLocalTrabalho)
        {
            try
            {
                var resultEacesso = _clienteService.ObterLocalTrabalhoEacesso(idLocalTrabalho);
                var resultVM = Mapper.Map<ClienteLocalTrabalhoEacessoVM>(resultEacesso);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("obter-contato-cliente-eacesso")]
        public IActionResult ObterContatoClienteEacesso(int idContato)
        {
            try
            {
                var resultEacesso = _clienteService.ObterContatoClienteEacesso(idContato);
                var resultVM = Mapper.Map<ContatoClienteEacessoVM>(resultEacesso);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public IActionResult Obtertodos()
        {
            try
            {
                var resultBD = _clienteService.ObterTodos();
                var resultVM = Mapper.Map<IEnumerable<ClienteVM>>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-por-id")]
        public IActionResult ObterPorId(int idCliente)
        {
            try
            {
                var resultBD = _clienteService.ObterPorId(idCliente);
                var resultVM = Mapper.Map<ClienteVM>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("somente-ativos")]
        public IActionResult SomenteAtivos()
        {
            try
            {
                var resultBD = _clienteService.ObterSomenteAtivos();
                var resultVM = Mapper.Map<IEnumerable<ClienteVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("somente-inativos")]
        public IActionResult SomenteInativos()
        {
            try
            {
                var resultBD = _clienteService.ObterSomenteInativos();
                var resultVM = Mapper.Map<IEnumerable<ClienteVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("ObterClientesPorIds")]
        public async Task<IActionResult> ObterTodosPorClientesIds([FromBody] List<int> ids)
        {
            try
            {
                var resultBD = await _clienteService.ObterClientesPorIds(ids);
                var resultVM = Mapper.Map<IEnumerable<MultiselectViewModel>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("{idCliente}/obter-cliente-migrado")]
        public async Task<IActionResult> ObterClienteMigrado(int idCliente)
        {
            var clienteExistente = _clienteService.VerificarExistenciaCliente(idCliente);
            if (clienteExistente)
            {
                return Ok(idCliente);
            }

            var pendente = false;
            var salesObject = new AccountSalesObject();
            var clienteEacessoService = new ClienteEacessoService(_connectionStrings.Value.EacessoConnection);
            var clienteEacesso = clienteEacessoService.ObterClienteEacessoPorId(idCliente);
            ClienteET clienteStfCorp = null;
            if (clienteEacesso != null)
            {
                salesObject = Mapper.Map(clienteEacesso, salesObject);
            }
            if (salesObject.CNPJ__c != null && salesObject.CNPJ__c.Trim() != "")
            {
                salesObject.CNPJ__c = _clienteService.ApenasNumeros(salesObject.CNPJ__c);
            }
            else
            {
                pendente = true;
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui cnpj", DescExcecao = "" });
                salesObject.CNPJ__c = _clienteService.ApenasNumeros("NWM9709244W4");
            }

            clienteStfCorp = Mapper.Map<ClienteET>(salesObject);
            if (clienteStfCorp.Id == 0)
            {
                clienteStfCorp.Id = _clienteService.VerificarIdCliente();
            }

            var endereco = Mapper.Map<Endereco>(salesObject);

            if (clienteStfCorp.NrTelefone != null)
            {
                clienteStfCorp.NrTelefone = clienteStfCorp.NrTelefone.Replace(" ", "");
                if (clienteStfCorp.NrTelefone.Contains('/'))
                {
                    var numeros = clienteStfCorp.NrTelefone.Split("/");
                    clienteStfCorp.NrTelefone = numeros[0];
                    clienteStfCorp.NrTelefone2 = numeros[0].Substring(0, 2) + numeros[1];
                }
            }
            if (clienteStfCorp.NrTelefone2 != null)
            {
                clienteStfCorp.NrTelefone = clienteStfCorp.NrTelefone2.Replace(" ", "");
            }

            if (clienteStfCorp.NmRazaoSocial == null || !clienteStfCorp.NmRazaoSocial.Any())
            {
                clienteStfCorp.NmRazaoSocial = "";
                pendente = true;
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui uma razão social", DescExcecao = "" });
            }
            if (clienteStfCorp.NmFantasia == null || !clienteStfCorp.NmFantasia.Any())
            {
                clienteStfCorp.NmFantasia = "";
                pendente = true;
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui um nome fantasia", DescExcecao = "" });
            }
            endereco.IdCidade = _enderecoService.VerificarEndereco(salesObject.CNPJ_Cidade__c);

            clienteStfCorp.Enderecos.Add(endereco);
            if (endereco.IdCidade == null)
            {
                pendente = true;
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui uma cidade valida", DescExcecao = "" });
            }

            if (!_clienteService.ValidarCNPJ(clienteStfCorp.NrCnpj, "BRASIL").Result)
            {
                pendente = true;
                await _logGenericoRepository.AddLog(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = "cliente: " + salesObject.Name + " não possui um cnpj valido", DescExcecao = "" });
            }

            if (pendente)
            {
                clienteStfCorp.FlStatus = "P";
            }
            else
            {
                clienteStfCorp.FlStatus = "A";
            }
            salesObject.IdStfCorp = _clienteService.PersistirCliente(clienteStfCorp);
            return Ok(salesObject.IdStfCorp);
        }

        [HttpGet]
        [Route("{nome}/obter-cliente-por-nome")]
        public IActionResult GetClientePorNome([FromRoute]string nome)
        {
            try
            {
                var resultBD = _clienteService.ObterClientesPorNome(nome);
                var resultVM = Mapper.Map<IEnumerable<ClienteVM>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("id-eacesso/{idEAcesso}")]
        public IActionResult ObterClientePorIdEacesso([FromRoute] int idEAcesso)
        {
            try
            {
                var resultBD = _clienteService.ObterClientePorIdEacesso(idEAcesso);
                var resultVM = Mapper.Map<ClienteVM>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
