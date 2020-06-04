using System;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Entity
{
    public class ClassificacaoContabil : EntityBase
    {
        public string DescClassificacaoContabil { get; set; }
        public string SgClassificacaoContabil { get; set; }
        public bool FlStatus { get; set; }
        public int? IdCategoriaContabil { get; set; }
        public virtual ICollection<PortfolioServico> PortifolioServicos { get; set; }
        public virtual CategoriaContabil CategoriaContabil { get; set; }
    }   
}
