using System;

namespace Cadastro.Api.ViewModels
{
    public class PeriodoLimiteVM
    {
        public int Id { get; set; }
        public int IdPeriodo { get; set; }
        public int IdDiaPagamento { get; set; }
        public string DescDiaPagamento { get; set; }
        public DateTime DiaLimiteLancamentoHoras { get; set; }
        public DateTime DiaLimiteAprovacaoHoras { get; set; }
        public DateTime DiaLimiteEnvioNF { get; set; }
    }
}
