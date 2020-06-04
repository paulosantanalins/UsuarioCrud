using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Entity
{
    public class DeParaServico : EntityBase
    {
        public int IdServicoEacesso { get; set; }
        public string DescStatus { get; set; }
        public int IdServicoContratado { get; set; }
        public string DescTipoServico { get; set; }
        public string NmServicoEacesso { get; set; }

        //novos campos
        public string DescSubTipoServico { get; set; }
        public string DescEscopo { get; set; }

        public virtual ServicoContratado ServicoContratado { get; set; }
    }
}
