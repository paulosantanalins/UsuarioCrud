using Microsoft.AspNetCore.Mvc;
using Utils;

namespace Account.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly IVariablesToken _variables;

        public HealthController(IVariablesToken variables)
        {
            _variables = variables;
        }

        [HttpGet]
        public IActionResult CheckHealth()
        {
            return Ok();
        }
    }
}
