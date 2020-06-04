using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public class FiltroComPeriodo<T>
    {
        public string ValorParaFiltrar { get; set; }
        public string FiltroGenerico { get; set; }
        public int Pagina { get; set; }
        public int QuantidadePorPagina { get; set; }
        public int Total { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public List<T> Valores { get; set; }
        public int Id { get; set; }
        public string OrdemOrdenacao { get; set; }
        public string CampoOrdenacao { get; set; }
    }
}
