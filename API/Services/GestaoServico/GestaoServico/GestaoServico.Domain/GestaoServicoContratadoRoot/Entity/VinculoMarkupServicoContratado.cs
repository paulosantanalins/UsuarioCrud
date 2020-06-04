using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Entity
{
    public class VinculoMarkupServicoContratado : EntityBase
    {
        public decimal VlMarkup { get; set; }
        public DateTime DtInicioVigencia { get; set; }
        public DateTime? DtFimVigencia { get; set; }
        public int IdServicoContratado { get; set; }

        public virtual ServicoContratado ServicoContratado { get; set; }
    }
}
