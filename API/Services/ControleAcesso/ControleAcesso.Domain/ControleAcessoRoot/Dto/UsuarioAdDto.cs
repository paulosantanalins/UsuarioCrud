using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.ControleAcessoRoot.Dto
{
    public class UsuarioAdDto
    {
        public string Login { get; set; }
        public string NomeCompleto { get; set; }
        public string Celula { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public string DataNascimento { get; set; }
        public string IdEacesso { get; set; }
    }
}
