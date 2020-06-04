using System;
using System.Collections.Generic;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class TransferenciaPrestador : EntityBase
    {
        public int IdPrestador { get; set; }
        public int Situacao { get; set; }
        public DateTime DataTransferencia { get; set; }
        public int? IdEmpresaGrupo { get; set; }
        public int? IdFilial { get; set; }
        public int? IdCelula { get; set; }
        public int? IdCliente { get; set; }
        public int? IdServico { get; set; }
        public int? IdLocalTrabalho { get; set; }

        public virtual Prestador Prestador { get; set; }

        public ICollection<LogTransferenciaPrestador> LogsTransferenciaPrestador { get; set; }
    }
}
