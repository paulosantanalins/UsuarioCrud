using Cadastro.Domain.SharedRoot;
using System;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ExtensaoContratoPrestador : ContratoBase
    {
        public virtual ContratoPrestador ContratoPrestador { get; set; }
        public int IdContratoPrestador { get; set; }
    }
}
