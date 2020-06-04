using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoServicoContratado
{
    public class DeParaViewModel
    {
        public int IdServicoEacesso { get; set; }
        public string NmServicoEacesso { get; set; }
        public string DescStatus { get; set; }
        public int IdServicoContratado { get; set; }
        public string DescTipoServico { get; set; }
        public ServicoContratadoVM ServicoContratado { get; set; }
    }
}
