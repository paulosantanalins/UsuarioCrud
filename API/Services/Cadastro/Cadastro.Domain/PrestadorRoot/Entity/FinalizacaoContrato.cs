using System;
using System.Collections;
using System.Collections.Generic;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class FinalizacaoContrato : EntityBase
    {
        public FinalizacaoContrato()
        {
            LogsFinalizacaoCntrato = new List<LogFinalizacaoContrato>();
        }

        public int IdPrestador { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataFimContrato { get; set; }
        public bool RetornoPermitido { get; set; }
        public string Motivo { get; set; }
        public int Situacao { get; set; }

        public virtual Prestador Prestador { get; set; }
        public virtual ICollection<LogFinalizacaoContrato> LogsFinalizacaoCntrato { get; set; }
    }
}
