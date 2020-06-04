using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;

namespace GestaoServico.Api.ViewModels.GestaoRespasse
{
    public class GridRepasseVM : ViewModelBase
    {
        public string CelOrigem { get; set; }
        public string CelDestino { get; set; }
        public string CliDestino { get; set; }
        public string PacDestino { get; set; }
        public decimal? ValRepasse { get; set; }
        public string FlStatus { get; set; }
        public DateTime DtRepasse { get; set; }

    }
}
