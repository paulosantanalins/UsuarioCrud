using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmpresaGrupoController : Controller
    {
        protected readonly IEmpresaGrupoService _empresaGrupoService;

        public EmpresaGrupoController(
            IEmpresaGrupoService empresaGrupoService)
        {
            _empresaGrupoService = empresaGrupoService;
        }

        [HttpGet("rm")]
        public IActionResult BuscarNoRM()
        {
            var result = _empresaGrupoService.BuscarNoRM();
            return Ok(result);
        }

        [HttpGet("rm/{id}")]
        public IActionResult BuscarNoRMPorId(int id)
        {
            var result = _empresaGrupoService.BuscarNoRMPorId(id, true, 1);
            return Ok(result);
        }

        [HttpGet("rm-todas")]
        public IActionResult BuscarTodasNoRM()
        {
            var result = _empresaGrupoService.BuscarTodasNoRM();
            return Ok(result);
        }
    }
}
