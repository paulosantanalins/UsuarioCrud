using Forecast.Domain.SharedRoot;
using System;

namespace Forecast.Domain.ForecastRoot
{
    public class ForecastET : EntityBaseCompose
    {
        public ForecastET()
        {
            ValorForecast = new ValorForecast();
        }
       
        public DateTime? DataAniversario { get; set; }
        public DateTime? DataAplicacaoReajuste { get; set; }
        public DateTime? DataReajusteRetroativo { get; set; }
        public bool FaturamentoNaoRecorrente { get; set; }
        public int? IdStatus { get; set; }
        public string DescricaoJustificativa { get; set; }        
        public virtual ValorForecast ValorForecast { get; set; }           
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
