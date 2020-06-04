using System;
using System.Collections.Generic;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Dto
{
    public class FiltroAprovarRepasseDto
    {
        public int TpRepasse { get; set; }
        public List<int> CelulasInteresse { get; set; }
        public DateTime? DtInicial { get; set; }
        public DateTime? DtFinal { get; set; }
        public int TpConsulta { get; set; }
        public bool AvaliarTodos { get; set; }
        public int Celula { get; set; }

        public int Pagina { get; set; }
        public int QuantidadePorPagina { get; set; }
        public int Total { get; set; }
        public List<GridRepasseAprovarDto> Valores { get; set; }

        public string OrdemOrdenacao { get; set; }
        public string CampoOrdenacao { get; set; }
    }
}
