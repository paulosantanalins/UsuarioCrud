using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers
{
    [Route("")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<AccountModel> Get()
        {
            return new []
            {
                new AccountModel { Id = Guid.NewGuid(), Subject = "Why Azure?", Body = "..." },
                new AccountModel { Id = Guid.NewGuid(), Subject = "Docker is Awesome", Body = "..." },
                new AccountModel { Id = Guid.NewGuid(), Subject = "Kubernetes is Amazing", Body = "..." }

            };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"Teste - {id}";
        }
    }

    public class AccountModel
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
