using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account.Api.ViewModels;
using Account.Domain.ProjetoRoot.Entity;
using Account.Domain.ProjetoRoot.Repository;
using Account.Domain.ProjetoRoot.Service.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Account.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Sistema")]
    public class SistemaController : Controller
    {
        private readonly ISistemaService _sistemaService;
        private readonly ISistemaRepository _sistemaRepository;

        public SistemaController(ISistemaService sistemaService,
                                 ISistemaRepository sistemaRepository)
        {
            _sistemaService = sistemaService;
            _sistemaRepository = sistemaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSistemas()
        {
            try
            {
                var projetos = await _sistemaRepository.ObterTodosSistemas();
                return await Task.Run(() => Ok(new
                {
                    dados = projetos,
                    notifications = "",
                    success = true
                }));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostSistema([FromBody] ProjetoViewModel projetoVM)
        {
            var sistema = Mapper.Map<Sistema>(projetoVM);
            await _sistemaService.PersistirSistema(sistema);
            return await Task.Run(() => Ok(new
            {
                dados = sistema,
                notifications = "",
                success = true
            }));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSistema([FromRoute] int id)
        {

            await _sistemaRepository.DeleteSistema(id);
            return await Task.Run(() => Ok(new
            {
                dados = "",
                notifications = "",
                success = true
            }));
        }

    }
}
