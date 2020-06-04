using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity
{
    public class PeriodoRepasse : EntityBase
    {
        public DateTime DtLancamentoInicio { get; set; }
        public DateTime DtLancamentoFim { get; set; }
        public DateTime DtAnaliseInicio { get; set; }
        public DateTime DtAnaliseFim { get; set; }
        public DateTime DtAprovacaoInicio { get; set; }
        public DateTime DtAprovacaoFim { get; set; }                
        public DateTime DtLancamento { get; set; }
    }
}
