using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguranca.Api.ViewModels
{
    public class InformacoesUsuarioViewModel
    {
        public string NomeCompleto { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Celula { get; set; }
        public string UidNumber { get; set; }
    }
}
