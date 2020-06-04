using Microsoft.AspNetCore.Mvc;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult CheckHealth()
        {
            return Ok();
        }
    }
}
