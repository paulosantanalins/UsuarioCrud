using Microsoft.AspNetCore.Mvc;
using Utils;

namespace Seguranca.Api.Controllers
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

        [HttpGet("ok")]
        public IActionResult CheckHealthOk()
        {
            return Ok("ok");
        }
    }
}
