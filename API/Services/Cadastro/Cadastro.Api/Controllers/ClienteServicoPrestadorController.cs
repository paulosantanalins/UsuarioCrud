using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using Utils.Connections;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ClienteServicoPrestadorController : Controller
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IClienteServicoPrestadorService _clienteServicoPrestadorService;
        private readonly IPrestadorService _prestadorService;

        public ClienteServicoPrestadorController(
            IOptions<ConnectionStrings> connectionStrings,
            IClienteServicoPrestadorService clienteServicoPrestadorService,
            IPrestadorService prestadorService)
        {
            _connectionStrings = connectionStrings;
            _clienteServicoPrestadorService = clienteServicoPrestadorService;
            _prestadorService = prestadorService;
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] ClienteServicoPrestadorVM clienteServicoPrestadorVM)
        {        
            SqlConnection eacessoConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection);
            eacessoConnection.Open();
            SqlTransaction eacessoTran = eacessoConnection.BeginTransaction();

            if (clienteServicoPrestadorVM.Id == 0)
            {
                var clienteServicoPrestador = Mapper.Map<ClienteServicoPrestador>(clienteServicoPrestadorVM);
                var clienteServicoPrestadorDB = _clienteServicoPrestadorService.Adicionar(clienteServicoPrestador);
                var prestador = _prestadorService.BuscarPorId(clienteServicoPrestador.IdPrestador);

                clienteServicoPrestador.Ativo = true;
                clienteServicoPrestadorVM.Id = clienteServicoPrestadorDB.Id;
                //Atualiza Célula Prestador STFCORP
                prestador.IdCelula = clienteServicoPrestador.IdCelula;
                _prestadorService.AtualizarPrestador(prestador);


                if (prestador.CodEacessoLegado != null)
                {
                    _prestadorService.AtualizarEAcesso(prestador.Id, eacessoConnection, eacessoTran);
                }
                _clienteServicoPrestadorService.InserirClienteServicoPrestadorEAcesso(clienteServicoPrestador, eacessoConnection, eacessoTran);
            }
            else
            {
                var clienteServicoPrestador = Mapper.Map<ClienteServicoPrestador>(clienteServicoPrestadorVM);
                _clienteServicoPrestadorService.Atualizar(clienteServicoPrestador);
                var prestador = _prestadorService.BuscarPorId(clienteServicoPrestador.IdPrestador);
                clienteServicoPrestador.Prestador = prestador;
                //Atualiza Célula Prestador 
                prestador.IdCelula = clienteServicoPrestador.IdCelula;
                _prestadorService.AtualizarPrestador(prestador);

                if (prestador.CodEacessoLegado != null)
                {
                    _prestadorService.AtualizarEAcesso(prestador.Id, eacessoConnection, eacessoTran);
                    _clienteServicoPrestadorService.AtualizarEAcesso(clienteServicoPrestador, eacessoConnection , eacessoTran);
                }
            }

            eacessoTran.Commit();        

            clienteServicoPrestadorVM = Mapper.Map<ClienteServicoPrestadorVM>(_clienteServicoPrestadorService.BuscarPorId(clienteServicoPrestadorVM.Id));

            return Ok(clienteServicoPrestadorVM);
        }

        [HttpGet("Inativar/{idClienteServicoPrestador}")]
        public IActionResult Inativar([FromRoute] int idClienteServicoPrestador)
        {
            _clienteServicoPrestadorService.Inativar(idClienteServicoPrestador);
            return Ok();
        }
    }
}
