using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using Cadastro.Domain.DominioRoot.Dto;
using Utils;
using System.Collections.Generic;
using System.Security.Claims;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/dominio")]
    public class DominioController : Controller
    {
        private readonly IDominioService _dominioService;

        public DominioController(
            IDominioService dominioService)
        {
            _dominioService = dominioService;
        }


        [HttpPost]
        public IActionResult Persistir([FromBody]DominioDto dominioDto)
        {
            var dominio = Mapper.Map<Dominio>(dominioDto);
            _dominioService.Persistir(dominio);
            return Ok();
        }

        [HttpPut]
        public IActionResult Editar([FromBody]DominioDto dominioDto)
        {
            var dominio = Mapper.Map<Dominio>(dominioDto);
            _dominioService.Editar(dominio);
            return Ok();
        }

        [HttpPut("MudarStatusDominio/{id}")]
        public IActionResult Inativar([FromRoute] int id)
        {
            _dominioService.MudarStatusDominio(id);
            return Ok();
        }
        
        [HttpPost("validar")]
        public IActionResult ValidarDominio([FromBody] DominioDto dominioDto)
        {
            var valido = _dominioService.ValidarDominio(dominioDto);
            return Ok(new { valido });
        }

        [HttpPost("filtrar")]
        public IActionResult FiltrarDominios([FromBody]FiltroGenericoDto<DominioDto> filtro)
        {
            var result = _dominioService.FiltrarDominios(filtro);
            return Ok(new { dados = result, notifications = "", success = true });
        }

        [HttpGet("{id}")]
        public IActionResult BuscarDominioPorId([FromRoute] int id)
        {
            var dominio = _dominioService.BuscarDominioPorId(id);
            return Ok(dominio);
        }

        [HttpGet("combo-dominios")]
        public IActionResult BuscarCombosGruposDominios()
        {
          
            var combos = _dominioService.BuscarCombosGruposDominios();
            return Ok(new { dados = combos, notifications = "", success = true });
        }

        [HttpGet("carregar-dominio/{tipoDominio}")]
        public IActionResult BuscarDominioPorDiscriminator(string tipoDominio)
        {
            var itens = _dominioService.BuscarItens(tipoDominio);
            return Ok(itens);
        }

        [HttpGet("carregar-dominio-propriedade/{tipoDominio}")]
        public IActionResult BuscarDominioPropriedade(string tipoDominio)
        {
            var itens = _dominioService.BuscarItensPropriedade(tipoDominio);
            return Ok(itens);
        }

        [HttpGet("todos/{tipoDominio}")]
        public IActionResult BuscarTodosDominioPorDiscriminator(string tipoDominio)
        {
            var itens = _dominioService.BuscarTodosItens(tipoDominio);
            return Ok(itens);
        }
    }
}
