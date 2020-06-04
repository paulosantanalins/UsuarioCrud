using AutoMapper;
using EnvioEmail.Api.ViewModels;
using EnvioEmail.Domain.EmailRoot.Dto;
using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;

namespace EnvioEmail.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TemplateEmailController : Controller
    {
        private readonly ITemplateEmailService _templateEmailService;

        public TemplateEmailController(ITemplateEmailService templateEmailService)
        {
            _templateEmailService = templateEmailService;
        }

        [HttpPost("persistir")]
        public IActionResult Persistir([FromBody] TemplateEmailVM templateEmailVM)
        {
            try
            {                
                var templateEmail = Mapper.Map<TemplateEmail>(templateEmailVM);                
                _templateEmailService.PersistirTemplateEmail(templateEmail);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("buscarPorId/{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            try
            {
                var resultBD = _templateEmailService.BuscarPorId(id);
                var resultVM = Mapper.Map<TemplateEmailVM>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("buscarPorIdComParametros/{id}")]
        public IActionResult BuscarPorIdComParametros([FromRoute] int id)
        {
            try
            {
                var resultBD = _templateEmailService.BuscarPorIdComParametors(id);
                var resultVM = Mapper.Map<TemplateEmailVM>(resultBD);
                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
      


        [HttpPost("filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoViewModelBase<TemplateEmailVM> filtro)
        {
            try
            {
                var filtroDTO = Mapper.Map<FiltroGenericoDtoBase<TemplateEmailDto>>(filtro);

                var templatesEmail = _templateEmailService.Filtrar(filtroDTO);

                var templateEmailVM = Mapper.Map<FiltroGenericoViewModelBase<TemplateEmailVM>>(templatesEmail);
                return Ok(templateEmailVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}
