using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;
using static Cadastro.Domain.SharedRoot.SharedEnuns;

namespace Cadastro.Domain.PrestadorRoot.Entity
{
    public class LogTransferenciaPrestador : EntityBase
    {        
        public SituacoesTransferenciaEnum Status { get; set; }
        public string MotivoNegacao { get; set; }
        public TransferenciaPrestador Transferencia { get; set; }
        public int IdTransferenciaPrestador { get; set; }


    }
}
