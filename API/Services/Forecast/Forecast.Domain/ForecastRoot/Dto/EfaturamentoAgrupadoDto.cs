using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forecast.Api.ViewModels
{
    public class EfaturamentoAgrupadoDto
    {
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public int Ano { get; set; }
        public Decimal ValorRecorrente { get; set; }
        public Decimal ValorRecorrenteNao { get; set; }
        public Decimal ValorRecorrenteNovasVendas { get; set; }
        public Decimal ValorRecorrentePerdas { get; set; }
        public Decimal ValorRecorrenteMultas { get; set; }
        public Decimal ValorRecorrenteRepactuacao { get; set; }
        public Decimal ValorRecorrenteRepactuacaoRetro { get; set; }
        public int? Mes { get; set; }
    }
}
