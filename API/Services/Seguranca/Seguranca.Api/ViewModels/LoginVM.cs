using System.ComponentModel.DataAnnotations;

namespace Seguranca.Api.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Aplicacao { get; set; }
        public string AplicacaoSenha { get; set; }
    }
}
