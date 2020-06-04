using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.SharedRoot;
using System;

namespace Cadastro.Domain.HorasMesRoot.Entity
{
    public class PeriodoDiaPagamento : EntityBase
    {
        public int IdPeriodo { get; set; }
        public int IdDiaPagamento { get; set; }
        public DateTime DiaLimiteLancamentoHoras { get; set; }
        public DateTime DiaLimiteAprovacaoHoras { get; set; } 
        public DateTime DiaLimiteEnvioNF { get; set; }
        public virtual HorasMes Periodo { get; set; }
        public virtual DomDiaPagamento DiaPagamento { get; set; }
    }
}
