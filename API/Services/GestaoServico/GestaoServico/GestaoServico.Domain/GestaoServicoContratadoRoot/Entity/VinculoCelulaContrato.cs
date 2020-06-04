using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Entity
{
    public class VinculoCelulaContrato : EntityBase
    {
        public VinculoCelulaContrato()
        {
            DeParaServicos = new HashSet<DeParaServico>();
        }
        public DateTime DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public string DescTipoCelula { get; set; }
        public int IdCelula { get; set; }
        public int? IdContrato { get; set; }
        public int? IdServicoContratado { get; set; }
        public int IdEscopoServico { get; set; }

        public virtual EscopoServico EscopoServico { get; set; } 
        public virtual ServicoContratado ServicoContratado { get; set; }
        public virtual Contrato Contrato { get; set; }
        public virtual ICollection<DeParaServico> DeParaServicos { get; set; }
    }
}
