using Cadastro.Domain.SharedRoot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ContratoPrestador : ContratoBase
    {   
        public virtual Prestador Prestador { get; set; }
        public int IdPrestador { get; set; }
        public ICollection<ExtensaoContratoPrestador> ExtensoesContratoPrestador { get; set; }
    }
}
