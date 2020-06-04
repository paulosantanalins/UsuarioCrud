using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Seguranca.Api.ViewModels
{
    public class LoginEmailVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Aplicacao { get; set; }
        public string AplicacaoSenha { get; set; }
        
    }
}
