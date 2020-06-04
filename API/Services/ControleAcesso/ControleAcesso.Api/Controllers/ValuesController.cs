using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Api.Controllers
{
    [Route("")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Values");
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"Teste - {id}";
        }
    }
}
