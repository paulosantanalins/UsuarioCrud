using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WinAuthentication.Api.Controllers
{
    [Authorize]
    [Route("api/Authentication/[controller]")]
	public class TransporteController : Controller
	{
        private readonly IConfiguration _configuration;
        public TransporteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var key = _configuration["Transporte:Chave"];
                var usuario = HttpContext.User.Identity.Name.ToString().Substring(HttpContext.User.Identity.Name.ToString().IndexOf("\\") + 1);
                var dominio = HttpContext.User.Identity.Name.ToString().Substring(0, HttpContext.User.Identity.Name.ToString().IndexOf("\\"));
                var encrypt = TransporteHash.EncryptString(usuario.ToUpper().Substring(0, 3) + DateTime.UtcNow.ToString("ddMMyyyyHH").ToString(), key);

                return Json(new TransporteHash { Usuario = usuario, Dominio = dominio, Chave = encrypt });
            }
            else
            {
                return Json(new TransporteHash { Usuario = null, Dominio = null, Chave = null });
            }
        }
       
    }
}
