using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.CelulaRoot.Entity
{
    public class Grupo : EntityBase
    {
        public string Descricao { get; set; }
        
        public bool Ativo { get; set; }

        public IEnumerable<Celula> Celulas { get; set; }
    }
}
