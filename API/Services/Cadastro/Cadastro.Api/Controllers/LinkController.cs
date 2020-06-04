using Cadastro.Domain.LinkRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Link")]
    public class LinkController : Controller
    {
        private readonly ILinkService _linkService;

        public LinkController(ILinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpGet("portal")]
        public IActionResult ObterLinkPortal()
        {
            var link = _linkService.ObterLinkPortal();
            return Ok(link);
        }
    }
}
