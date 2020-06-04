using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Entity
{
    public class CategoriaContabil : EntityBase
    {
        public string DescCategoria { get; set; }
        public string SgCategoriaContabil { get; set; }
        public bool FlStatus { get; set; }

        public virtual ICollection<ClassificacaoContabil> ClassificacoesContabil { get; set; }
    }


}
