using Microsoft.AspNetCore.Mvc;
using Utils;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult CheckHealth()
        {
            return Ok(Variables.EnvironmentName);
        }

        [HttpGet("teste")]
        public IActionResult Teste()
        {
            var b = 0;
            var a = 1 / b;
            return Ok(Variables.EnvironmentName);
        }
    }
}
