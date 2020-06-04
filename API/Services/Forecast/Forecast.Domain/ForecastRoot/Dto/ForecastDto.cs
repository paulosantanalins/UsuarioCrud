using System;
using System.Collections.Generic;
using System.Text;

namespace Forecast.Domain.ForecastRoot.Dto
{
    public class ForecastDto
    {
        public int Id { get; set; }
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public int IdServico { get; set; }
        public string NomeServico { get; set; }
        public int NrAno { get; set; }
        public DateTime? DataAniversario { get; set; }
        public DateTime? DataAplicacaoReajuste { get; set; }
        public DateTime? DataReajusteRetroativo { get; set; }
        public decimal? VlPercentual { get; set; }
        public int? IdStatus { get; set; }
        public string DescricaoJustificativa { get; set; }
        public bool FaturamentoNaoRecorrente { get; set; }
        public decimal? ValorJaneiro { get; set; }
        public decimal? ValorFevereiro { get; set; }
        public decimal? ValorMarco { get; set; }
        public decimal? ValorAbril { get; set; }
        public decimal? ValorMaio { get; set; }
        public decimal? ValorJunho { get; set; }
        public decimal? ValorJulho { get; set; }
        public decimal? ValorAgosto { get; set; }
        public decimal? ValorSetembro { get; set; }
        public decimal? ValorOutubro { get; set; }
        public decimal? ValorNovembro { get; set; }
        public decimal? ValorDezembro { get; set; }
        public decimal? ValorTotal { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool? AlertaDataAniversario { get; set; }
        public bool PassouDoisDiasViradaMes { get; set; } = true;
    }
}
