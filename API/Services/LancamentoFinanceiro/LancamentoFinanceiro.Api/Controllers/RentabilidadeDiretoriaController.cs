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
    [Route("api/rentabilidade-diretoria")]
    public class RentabilidadeDiretoriaController : Controller
    {
        protected readonly IRentabilidadeDiretoriaService _rentabilidadeDiretoriaService;

        public RentabilidadeDiretoriaController(IRentabilidadeDiretoriaService rentabilidadeDiretoriaService)
        {
            _rentabilidadeDiretoriaService = rentabilidadeDiretoriaService;
        }

        [HttpGet("gerar-relatorio-pdf")]
        public IActionResult PesquisaRentabilidadeCelula(FiltroRelatorioRentabilidadeDiretoriaVM filtroRelatorio)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroRelatorioRentabilidadeDiretoriaDto>(filtroRelatorio);
                return Ok(_rentabilidadeDiretoriaService.ObterInformacoesPorDiretoria(filtroDto));
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
