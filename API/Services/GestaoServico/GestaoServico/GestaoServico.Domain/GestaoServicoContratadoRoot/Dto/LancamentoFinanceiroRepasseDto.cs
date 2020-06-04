using System;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Dto
{
    public class LancamentoFinanceiroRepasseDto
    {
        public LancamentoFinanceiroRepasseDto()
        {
            ItensLancamentoFinanceiro = new HashSet<ItemLancamentoFinanceiroRepasseDto>();
        }

        public int Id { get; set; }
        public string DescricaoOrigemLancamento { get; set; }
        public DateTime DtLancamento { get; set; }
        public DateTime DtBaixa { get; set; }
        public string DescricaoTipoLancamento { get; set; }
        public int? IdLan { get; set; }
        public string CodigoColigada { get; set; }
        public DateTime? DtAlteracao { get; set; }
        public string LgUsuario { get; set; }
        public int? IdTipoDespesa { get; set; }
        public string DescOrigemCompraEacesso { get; set; }

        public ICollection<ItemLancamentoFinanceiroRepasseDto> ItensLancamentoFinanceiro { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
