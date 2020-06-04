using System;

namespace RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto
{
    public class PeriodoRepasseDto
    {
        public int Id { get; set; }
        public int AnoLancamento { get; set; }
        public int MesLancamento { get; set; }
        public bool? PeriodoVigente { get; set; }
        public bool ExisteRepasseNessePeriodo { get; set; }
        public bool EhAlteracaoCronograma { get; set; }
        public string Usuario { get; set; }
        public DateTime DtLancamentoInicio { get; set; }
        public DateTime DtLancamentoFim { get; set; }
        public DateTime DtAnaliseInicio { get; set; }
        public DateTime DtAnaliseFim { get; set; }
        public DateTime DtAprovacaoInicio { get; set; }
        public DateTime DtAprovacaoFim { get; set; }
        public DateTime? DtReferencia { get; set; }
        public DateTime? DtLancamento { get; set; }
        public DateTime? DataAlteracao;
    }
}
