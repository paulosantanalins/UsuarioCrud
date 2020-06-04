using System;
using System.Collections.Generic;

namespace LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity
{
    public class Contrato : EntityBase
    {
        public DateTime? DtFinalizacao { get; set; }
        public string DescContrato { get; set; }
        public DateTime DtInicial { get; set; }
        public string DescStatusSalesForce { get; set; }
        public int IdMoeda { get; set; }
        public string NrAssetSalesForce { get; set; }

        public virtual ICollection<ServicoContratado> ServicoContratados { get; set; }
        public virtual ICollection<ClienteContrato> ClientesContratos { get; set; }
    }
}
