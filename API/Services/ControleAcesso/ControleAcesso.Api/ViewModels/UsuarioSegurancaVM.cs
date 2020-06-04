using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels
{
    public class UsuarioSegurancaVM
    {
        public string Login { get; set; }
        public string NomeCompleto { get; set; }
        public string Celula { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
    }
}
