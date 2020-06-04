using Forecast.Domain.ForecastRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forecast.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Migracao")]
    public class MigracaoController : Controller
    {
        private readonly IForecastService _forecastService;

        public MigracaoController(
            IForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        [HttpGet("realizar-migracao/forecast/{senha}")]
        public IActionResult RealizarMigracaoForecast([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _forecastService.RealizarMigracao();
                return Ok("Migração Forecast realizada com sucesso");
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("realizar-migracao/forecast-bi/{senha}")]
        public IActionResult RealizarMigracaoForecastBi([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _forecastService.RealizarMigracaoBi();
                return Ok("Migração Forecast BI realizada com sucesso");
            }
            else
            {
                return Unauthorized();
            }
        }


    }
}
