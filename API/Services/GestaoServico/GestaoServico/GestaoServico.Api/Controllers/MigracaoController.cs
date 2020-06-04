using AutoMapper;
using GestaoServico.Api.Jobs;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MigracaoController : Controller
    {
        private readonly IRepasseMigracaoService _repasseMigracaoService;

        public MigracaoController(
            IRepasseMigracaoService repasseMigracaoService)
        {
            _repasseMigracaoService = repasseMigracaoService;
        }

        [HttpGet("repasse-eacesso/{dtInicio}/{dtFim}")]
        public IActionResult RepasseEacesso(string dtInicio, string dtFim)
        {
            var repassesEacesso = _repasseMigracaoService.BuscarRepassesEAcesso(dtInicio, dtFim);
            var repasses = Mapper.Map<List<Repasse>>(repassesEacesso);

            _repasseMigracaoService.MigrarRepassesEacesso(repasses);

            return Ok();
        }
    }
}
