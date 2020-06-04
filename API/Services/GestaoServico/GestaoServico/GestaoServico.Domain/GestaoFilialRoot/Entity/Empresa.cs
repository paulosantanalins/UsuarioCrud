using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoFilialRoot.Entity
{
    public class Empresa : EntityBase
    {
        public string NmEmpresa { get; set; }
        public string NmRazaoSocial { get; set; }
        public virtual ICollection<ServicoContratado> ServicosContratados { get; set; }
    }
}
