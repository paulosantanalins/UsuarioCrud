using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;
using Utils.Connections;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmpresaPrestadorController : Controller
    {
        private readonly IEmpresaService _empresaService;
        private readonly IEmpresaPrestadorService _empresaPrestadorService;
        private readonly IEmpresaPrestadorRepository _empresaPrestadorRepository;
        private readonly IPrestadorService _prestadorService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IPluginRMService _pluginRMService;
        private readonly IVariablesToken _variables;

        public EmpresaPrestadorController(
            IEmpresaService empresaService,
            IPluginRMService pluginRMService,
            IOptions<ConnectionStrings> connectionStrings,
            IEmpresaPrestadorService empresaPrestadorService,
            IEmpresaPrestadorRepository empresaPrestadorRepository,
            IPrestadorService prestadorService, IVariablesToken variables)
        {
            _empresaService = empresaService;
            _pluginRMService = pluginRMService;
            _connectionStrings = connectionStrings;
            _empresaPrestadorRepository = empresaPrestadorRepository;
            _empresaPrestadorService = empresaPrestadorService;
            _prestadorService = prestadorService;
            _variables = variables;
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            var resultBD = _empresaService.BuscarPorId(id);
            var resultVM = Mapper.Map<EmpresaVM>(resultBD);
            return Ok(new { dados = resultVM, notifications = "", success = true });
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] EmpresaVM empresaVM)
        {
            if (empresaVM.Id == 0)
            {
                var empresaPrestador = Mapper.Map<EmpresaPrestador>(empresaVM);
                empresaPrestador.Empresa.Usuario = _variables.UsuarioToken;
                empresaPrestador.Empresa.Ativo = true;
                var empresaDB = _empresaPrestadorService.Adicionar(empresaPrestador);
                empresaVM.Id = empresaDB.Id;
                empresaVM.IdEndereco = empresaDB.IdEndereco;
                var empresa = Mapper.Map<Empresa>(empresaVM);
                var prestador = _prestadorService.BuscarPorId(empresaVM.IdPrestador);
                _empresaService.AdicionarEmpresaDoPrestadorEAcesso(empresa, prestador.CodEacessoLegado.Value, prestador);
            }
            else
            {
                var empresa = Mapper.Map<Empresa>(empresaVM);
                CriarVinculoEmpresaPrestador(empresaVM, empresa);
                _empresaService.AtualizarEmpresaPrestador(empresa);
                _empresaService.AtualizarEmpresaDoPrestadorEAcesso(empresa, empresaVM.IdPrestador);
            }

            IntegrarComRM(empresaVM);

            empresaVM = Mapper.Map<EmpresaVM>(_empresaService.BuscarPorId(empresaVM.Id));

            return Ok(empresaVM);
        }

        private void CriarVinculoEmpresaPrestador(EmpresaVM empresaVM, Empresa empresa)
        {
            var existeVinculo = _empresaPrestadorRepository.Buscar(x => x.IdPrestador == empresaVM.IdPrestador && x.IdEmpresa == empresaVM.Id).Any();
            if (!existeVinculo)
            {
                _empresaPrestadorRepository.Adicionar(new EmpresaPrestador
                {
                    IdEmpresa = empresa.Id,
                    IdPrestador = empresaVM.IdPrestador,
                    DataAlteracao = DateTime.Now,
                    Usuario = _variables.UserName
                });
            }
        }

        private void IntegrarComRM(EmpresaVM empresaVM)
        {
            var connectionStringRM = _connectionStrings.Value.RMIntegracaoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                dbConnection.Open();
                _pluginRMService.EnviarEmpresaRM(empresaVM.IdPrestador, dbConnection, null);
                dbConnection.Close();
            }
        }

        [HttpGet("Inativar/{idEmpresa}/{idPrestador}")]
        public IActionResult Inativar([FromRoute] int idEmpresa, int idPrestador)
        {
            _empresaService.Inativar(idEmpresa);
            var empresa = _empresaService.BuscarPorId(idEmpresa);
            _empresaService.AtualizarEmpresaDoPrestadorEAcesso(empresa, idPrestador);
            return Ok(new { dados = "", notifications = "", success = true });
        }

        [HttpGet("cnpj/{cnpj}")]
        public IActionResult BuscarDadosEmpresa([FromRoute] string cnpj)
        {
            EmpresaVM empresaVM = null;
            var resultBD = _empresaService.BuscarPorCnpj(cnpj);
            if (resultBD != null)
            {
                empresaVM = Mapper.Map<EmpresaVM>(resultBD);
                empresaVM.Ativo = true;
            }
            return Ok(new { dados = empresaVM, notifications = "", success = true });
        }
    }
}
