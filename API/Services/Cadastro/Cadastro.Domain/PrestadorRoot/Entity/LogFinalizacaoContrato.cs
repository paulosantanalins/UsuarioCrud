using System;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class LogFinalizacaoContrato : EntityBase
    {
        public int IdFinalizacaoContrato { get; set; }
        public DateTime DataFimContrato { get; set; }
        public bool RetornoPermitido { get; set; }
        public string Motivo { get; set; }
        public string MotivoCancelamento { get; set; }
        public int Situacao { get; set; }
        public int Acao { get; set; }

        public virtual FinalizacaoContrato FinalizacaoContrato { get; set; }
    }
}
