using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EnderecoController : Controller
    {
        protected readonly IEnderecoService _enderecoService;

        public EnderecoController(
        IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpGet("abreviaturaLogradouro")]
        public IActionResult BuscarTodasAbreviaturaLogradouros()
        {
            var resultBD = _enderecoService.BuscarAbreviaturaLogradouro();
            IEnumerable<AbreviaturaLogradouroVM> resultVM = Mapper.Map<IEnumerable<AbreviaturaLogradouroVM>>(resultBD);

            return Ok(new { dados = resultVM, notifications = "", success = true });
        }

        [HttpGet("siglaPaisPorIdPais")]
        public IActionResult BuscarPaisPorId(int idPais)
        {
            var sigla = _enderecoService.BuscarPaisPorId(idPais).SgPais;
            return Ok(sigla);
        }
    }
}
