using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Entity
{
    public class VinculoServicoCelulaComercial : EntityBase
    {
        public int IdServicoContratado { get; set; }
        public int IdCelulaComercial { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

        public virtual ServicoContratado ServicoContratado { get; set; }
        public virtual Celula Celula { get; set; }
    }
}
