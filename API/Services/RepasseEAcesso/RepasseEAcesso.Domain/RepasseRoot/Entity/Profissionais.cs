using RepasseEAcesso.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepasseEAcesso.Domain.RepasseRoot.Entity
{
    public class Profissionais : EntityBase
    {
        
        public string CPF { get; set; }
        public string Nome { get; set; }
        public int Celula { get; set; }
    }
}
