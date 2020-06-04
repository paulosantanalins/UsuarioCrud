using System;
using System.Collections.Generic;

namespace GestaoServico.Api.ViewModels.GestaoRespasse
{
    public class FiltroAprovarRepasseVM
    {
        public int TpRepasse { get; set; }
        public string CelulasInteresse { get; set; }
        public string DtInicial { get; set; }
        public string DtFinal { get; set; }
        public int TpConsulta { get; set; }
        public bool AvaliarTodos { get; set; }
        public int Celula { get; set; }

        public int Pagina { get; set; }
        public int QuantidadePorPagina { get; set; }
        public int Total { get; set; }
        public List<GridRepasseAprovarVM> Valores { get; set; }

        public string OrdemOrdenacao { get; set; }
        public string CampoOrdenacao { get; set; }
    }
}
