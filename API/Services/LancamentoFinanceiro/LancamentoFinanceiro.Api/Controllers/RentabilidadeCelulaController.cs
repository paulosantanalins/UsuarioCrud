using AutoMapper;
using LancamentoFinanceiro.Api.ViewModels.Rentabilidade;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LancamentoFinanceiro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/rentabilidade-celula")]
    public class RentabilidadeCelulaController : Controller
    {
        protected readonly IRentabilidadeCelulaService _rentabilidadeCelulaService;

        public RentabilidadeCelulaController(IRentabilidadeCelulaService rentabilidadeCelulaService)
        {
            _rentabilidadeCelulaService = rentabilidadeCelulaService;
        }


        [HttpGet("servico-projeto")]
        public IActionResult PesquisaRentabilidadeCelula(FiltroRelatorioRentabilidadeCelulaVM filtroRelatorio)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroRelatorioRentabilidadeCelulaDto>(filtroRelatorio);
                return Ok(_rentabilidadeCelulaService.ObterInformacoesPorServicoProjeto(filtroDto));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
