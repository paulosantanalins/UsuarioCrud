using System;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class LogReajusteContrato : EntityBase
    {
        public int IdReajusteContrato { get; set; }
        public int IdTipoContrato { get; set; }
        public int QuantidadeHorasContrato { get; set; }
        public decimal ValorContrato { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataReajuste { get; set; }
        public int Situacao { get; set; }
        public int Acao { get; set; }
        public string Motivo { get; set; }

        public ReajusteContrato ReajusteContrato { get; set; }
    }
}
