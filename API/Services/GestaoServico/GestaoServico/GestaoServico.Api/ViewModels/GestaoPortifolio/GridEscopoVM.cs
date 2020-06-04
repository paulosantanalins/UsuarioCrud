using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.ViewModels.GestaoPortifolio
{
    public class GridEscopoVM
    {
        public int Id { get; set; }
        public string NmEscopoServico { get; set; }
        public bool FlAtivo { get; set; }
        public string NmTipoReembolso { get; set; }
    }
}
