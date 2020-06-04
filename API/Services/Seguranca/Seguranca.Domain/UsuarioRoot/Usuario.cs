using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Seguranca.Domain.UsuarioRoot
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; }
    }
}
