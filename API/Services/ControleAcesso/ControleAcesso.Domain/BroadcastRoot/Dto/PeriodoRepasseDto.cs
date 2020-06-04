using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.BroadcastRoot.Dto
{
    public class PeriodoRepasseDto
    {
        public DateTime DtLancamentoInicio { get; set; }
        public DateTime DtLancamentoFim { get; set; }
        public DateTime DtAnaliseInicio { get; set; }
        public DateTime DtAnaliseFim { get; set; }
        public DateTime DtAprovacaoInicio { get; set; }
        public DateTime DtAprovacaoFim { get; set; }
        public DateTime DtLancamento { get; set; }
        public bool ehAlteracaoCronograma { get; set; }
    }
}
