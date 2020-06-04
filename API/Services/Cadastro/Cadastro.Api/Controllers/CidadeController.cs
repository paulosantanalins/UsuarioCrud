using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.CidadeRoot.Service.Interfaces;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.EnderecoRoot.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Utils.Base;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CidadeController : Controller
    {
        private readonly ICidadeService _cidadeService;
        private readonly IEnderecoService _enderecoService;

        public CidadeController(ICidadeService cidadeService, IEnderecoService enderecoService)
        {
            _cidadeService = cidadeService;
            _enderecoService = enderecoService;
        }

        [HttpPost("filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoViewModelBase<CidadeGridVM> filtro)
        {
            var filtroDTO = Mapper.Map<FiltroGenericoDtoBase<CidadeDto>>(filtro);
            var resultBD = _cidadeService.Filtrar(filtroDTO);
            var resultVM = Mapper.Map<FiltroGenericoViewModelBase<CidadeGridVM>>(resultBD);
            return Ok(resultVM);
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] CidadeVM cidadeVM)
        {
            var cidade = Mapper.Map<Cidade>(cidadeVM);
            _cidadeService.PersistirCidade(cidade);
            return Ok(true);
        }


        [HttpGet("estados")]
        public IActionResult BuscarEstados()
        {
            var resultBD = _enderecoService.BuscarEstados();
            var resultVM = Mapper.Map<IEnumerable<ComboDefaultVM>>(resultBD);
            return Ok(resultVM);
        }

        [HttpGet("estadosDeUmPais/{id}")]
        public IActionResult BuscarEstadosDeUmPais(int id)
        {
            var resultBD = _enderecoService.BuscarEstadosDeUmPais(id);
            var resultVM = Mapper.Map<IEnumerable<ComboDefaultVM>>(resultBD);
            return Ok(resultVM);
        }

        [HttpGet("paises")]
        public IActionResult BuscarPaises()
        {
            var resultBD = _cidadeService.BuscarPaises();
            var resultVM = Mapper.Map<IEnumerable<ComboDefaultVM>>(resultBD);
            return Ok(resultVM);
        }

        [HttpGet("pais/{idPais}")]
        public IActionResult BuscarPaisPorId(int idPais)
        {
            var resultBD = _enderecoService.BuscarPaisPorId(idPais);
            var resultVM = Mapper.Map<PaisVM>(resultBD);
            return Ok(resultVM);
        }



        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            var resultBD = _cidadeService.BuscarPorId(id);
            var resultVM = Mapper.Map<CidadeVM>(resultBD);
            return Ok(new { dados = resultVM, notifications = "", success = true });
        }

        [HttpGet("obter-por-estado/{idEstado}")]
        public IActionResult BuscarCidadesPeloEstado(int idEstado)
        {
            var resultBD = _enderecoService.BuscarCidadesPeloEstado(idEstado);
            var resultVM = Mapper.Map<IEnumerable<ComboDefaultVM>>(resultBD);
            return Ok(resultVM);
        }

    }
}
