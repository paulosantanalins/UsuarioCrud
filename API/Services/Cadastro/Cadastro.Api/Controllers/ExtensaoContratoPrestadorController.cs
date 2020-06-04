using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ExtensaoContratoPrestadorController : Controller
    {
        private readonly IExtensaoContratoPrestadorService _extensaoContratoPrestadorService;

        public ExtensaoContratoPrestadorController(IExtensaoContratoPrestadorService extensaoContratoPrestadorService)
        {
            _extensaoContratoPrestadorService = extensaoContratoPrestadorService;
        }             

        [HttpPost]
        public IActionResult Persitir([FromBody] ExtensaoContratoPrestadorVM extensaoContratoPrestadorVM)
        {
            var extensaoContratoPrestador = Mapper.Map<ExtensaoContratoPrestador>(extensaoContratoPrestadorVM);
            _extensaoContratoPrestadorService.Persistir(extensaoContratoPrestador, extensaoContratoPrestadorVM.ArquivoBase64);
            return Ok();
        }

        [HttpPut("inativar-extensao-contrato-prestador")]
        public IActionResult InativarExtensaoContratoPrestador([FromBody] int idExtensaoContratoPrestador)
        {
            _extensaoContratoPrestadorService.InativarExtensaoContratoPrestador(idExtensaoContratoPrestador);
            return Ok();
        }
    }
}
