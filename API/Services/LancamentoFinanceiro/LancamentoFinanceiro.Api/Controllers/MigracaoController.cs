using System;
using AutoMapper;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LancamentoFinanceiro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MigracaoController : Controller
    {
        private readonly IMigracaoRMService _migracaoRMService;

        public MigracaoController(
            IMigracaoRMService migracaoRMService)
        {
            _migracaoRMService = migracaoRMService;
        }

        [HttpGet("lancamentos-rm/{dtInicio}/{dtFim}")]
        public IActionResult LancamentosRM(string dtInicio, string dtFim)
        {
            var lancamentos = _migracaoRMService.BuscarLancamentosFinanceirosRM(dtInicio, dtFim);
            var lancamentoCount = lancamentos.Count;
            var rootLancamentos = Mapper.Map<List<RootLancamentoFinanceiro>>(lancamentos);

            var vezes = 0;
            const int take = 50;
            var lancamentosTake = rootLancamentos.Skip(0).Take(take).ToList();
            while (lancamentosTake.Any())
            {
                _migracaoRMService.MigrarLancamentosFinanceirosRM(lancamentosTake);
                vezes++;
                lancamentosTake = rootLancamentos.Skip(vezes * take).Take(take).ToList();
            }

            return Ok();
        }
    }
}
