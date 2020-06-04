using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Utils.Base;

namespace ControleAcesso.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class GrupoController : Controller
    {
        protected readonly IGrupoService _grupoService;

        public GrupoController(
            IGrupoService GrupoService)
        {
            _grupoService = GrupoService;
        }

        [HttpPost("filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoViewModelBase<GrupoVM> filtro)
        {
            try
            {
                var filtroDTO = Mapper.Map<FiltroGenericoDtoBase<GrupoDto>>(filtro);
                var resultBD = _grupoService.Filtrar(filtroDTO);
                var resultVM = Mapper.Map<FiltroGenericoViewModelBase<GrupoVM>>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("{id}")]
        public IActionResult BuscarGrupo(int id)
        {
            try
            {
                var resultBD = _grupoService.BuscarPorId(id);
                var resultVM = Mapper.Map<GrupoVM>(resultBD);
                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        

        [HttpPost]
        public IActionResult Persistir([FromBody] GrupoVM grupoVM)
        {
            try
            {
                var validarGrupoExiste = _grupoService.ValidarExisteGrupo(grupoVM.Descricao);

                if (validarGrupoExiste)
                {
                    return Ok(false);
                }

                var grupo = Mapper.Map<Grupo>(grupoVM);
                if (grupo.Id == 0)
                {
                    grupo.Ativo = true;
                    _grupoService.SalvarGrupo(grupo);
                }
                else
                {
                    _grupoService.AtualizarGrupo(grupo);
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("inativar/{id}")]
        public IActionResult Inativar(int id)
        {
            try
            {                
                _grupoService.Inativar(id);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("reativar/{id}")]
        public IActionResult Reativar(int id)
        {
            try
            {
                _grupoService.Reativar(id);
                return Ok(new { dados = "", notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("verificar-existe-grupo")]
        public IActionResult VerificarExisteGrupoComCelulasInativar([FromBody] int id)
        {
            try
            {
                var existeGrupoComCelulasInativas = _grupoService.ExisteGrupoComCelulasInativas(id);
               
                return Ok(existeGrupoComCelulasInativas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("BuscarTodos")]
        public IActionResult BuscarTodos()
        {
            try
            {
                var resultBD = _grupoService.BuscarTodos();
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("BuscarTodosAtivos")]
        public IActionResult BuscarTodosAtivos()        
        {
            try
            {
                var resultBD = _grupoService.BuscarTodosAtivos();
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
