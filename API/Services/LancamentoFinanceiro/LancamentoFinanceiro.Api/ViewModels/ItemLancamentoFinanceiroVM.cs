using System;

namespace LancamentoFinanceiro.Api.ViewModels
{
    public class ItemLancamentoFinanceiroVM
    {
        public int IdLancamentoFinanceiro { get; set; }
        public int? IdServicoContratado { get; set; }
        public decimal VlLancamento { get; set; }
        public int? IdRepasse { get; set; }
        public DateTime? DtRepasse { get; set; }
        public DateTime? DtAlteracao { get; set; }
        public string LgUsuario { get; set; }
        public decimal? VlDesc { get; set; }
        public decimal? VlInc { get; set; }
    }
}
