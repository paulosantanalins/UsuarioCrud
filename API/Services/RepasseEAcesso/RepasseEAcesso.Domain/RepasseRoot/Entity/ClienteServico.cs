using RepasseEAcesso.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Domain.RepasseRoot.Entity
{
    public class ClienteServico : EntityBase
    {
        public string Nome { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
    }
}
