using AutoMapper;
using EnvioEmail.Api.ViewModels;
using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EnvioEmail.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Email")]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] EmailVM emailVM)
        {
            var emailBD = Mapper.Map<Email>(emailVM);
            var email = _emailService.Enviar(emailBD);

            if (email != null) return Ok(email);

            return Ok();
        }
    }
}
