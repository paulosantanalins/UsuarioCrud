
using System;
using System.Collections.Generic;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class ReajusteContrato : EntityBase
    {
        public ReajusteContrato()
        {
            LogsReajusteContratos = new List<LogReajusteContrato>();
        }

        public int IdPrestador { get; set; }
        public int IdTipoContrato { get; set; }
        public int QuantidadeHorasContrato { get; set; }
        public decimal ValorContrato { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataReajuste { get; set; }
        public int Situacao { get; set; }

        public virtual Prestador Prestador { get; set; }
        public virtual ICollection<LogReajusteContrato> LogsReajusteContratos { get; set; }
    }
}
