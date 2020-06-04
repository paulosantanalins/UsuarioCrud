

using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.DominioRoot.Entity
{
    public class Dominio : EntityBase
    {
        public string ValorTipoDominio { get; set; }
        public int IdValor { get; set; }
        public string DescricaoValor { get; set; }
        public bool Ativo { get; set; }
    }
}
