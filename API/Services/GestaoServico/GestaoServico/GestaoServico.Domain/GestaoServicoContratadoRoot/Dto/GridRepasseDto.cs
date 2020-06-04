using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Dto
{
    public class GridRepasseDto : DtoBase
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
