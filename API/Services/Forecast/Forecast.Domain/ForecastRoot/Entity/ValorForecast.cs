using Forecast.Domain.ForecastRoot.Attributes;
using Forecast.Domain.SharedRoot;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forecast.Domain.ForecastRoot
{
    public class ValorForecast : EntityBaseCompose
    {       
        [MonthNumber(1)]
        public decimal? ValorJaneiro { get; set; }
        [MonthNumber(2)]
        public decimal? ValorFevereiro { get; set; }
        [MonthNumber(3)]
        public decimal? ValorMarco { get; set; }
        [MonthNumber(4)]
        public decimal? ValorAbril { get; set; }
        [MonthNumber(5)]
        public decimal? ValorMaio { get; set; }
        [MonthNumber(6)]
        public decimal? ValorJunho { get; set; }
        [MonthNumber(7)]
        public decimal? ValorJulho { get; set; }
        [MonthNumber(8)]
        public decimal? ValorAgosto { get; set; }
        [MonthNumber(9)]
        public decimal? ValorSetembro { get; set; }
        [MonthNumber(10)]
        public decimal? ValorOutubro { get; set; }
        [MonthNumber(11)]
        public decimal? ValorNovembro { get; set; }
        [MonthNumber(12)]
        public decimal? ValorDezembro { get; set; }
        public decimal? ValorAjuste { get; set; }
        public decimal? VlPercentual { get; set; }
        public virtual ForecastET Forecast { get; set; }
        [NotMapped]
        public decimal? ValorTotal { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
