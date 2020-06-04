using System;
using System.Collections.Generic;
using System.Text;
using static RepasseEAcesso.Domain.SharedRoot.SharedEnuns;

namespace RepasseEAcesso.Domain.RepasseRoot.Dto
{
    public class FiltroRepasseNivelUmDto<T>
    {
        public string ValorParaFiltrar { get; set; }
        public string FiltroGenerico { get; set; }
        public int Pagina { get; set; }
        public int QuantidadePorPagina { get; set; }
        public int[] CelulasEscolhidas { get; set; }
        public StatusRepasseEacesso? Status { get; set; }
        public int IdPeriodoRepasse { get; set; }
        public int Total { get; set; }
        public List<T> Valores { get; set; }
        public int Id { get; set; }

        public string OrdemOrdenacao { get; set; }
        public string CampoOrdenacao { get; set; }
    }
}
