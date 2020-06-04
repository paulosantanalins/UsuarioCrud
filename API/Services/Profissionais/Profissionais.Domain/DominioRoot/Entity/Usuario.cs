using UsuarioApi.Domain.SharedRoot.Entity;
using System;

namespace UsuarioApi.Domain.DominioRoot.Entity
{
    public  class Usuario : EntityBase
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public DateTime DataNasc { get; set; }
        public int Escolaridade { get; set; }
    }
}
