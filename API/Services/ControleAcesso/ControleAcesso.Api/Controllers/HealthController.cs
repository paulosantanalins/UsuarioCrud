using Microsoft.AspNetCore.Mvc;
using Utils;

namespace ControleAcesso.Api.Controllers
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
    }
}
