using System;

namespace UsuarioApi.Domain.DominioRoot.Dto
{
    public class UsuarioDto
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public DateTime DataNasc { get; set; }
        public int Escolaridade { get; set; }
    }
}
