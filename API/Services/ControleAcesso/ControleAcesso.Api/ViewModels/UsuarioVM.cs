using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.ViewModels
{
    public class UsuarioVM
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
