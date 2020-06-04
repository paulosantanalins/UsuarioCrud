using System;
using System.Collections.Generic;
using System.Text;

namespace Forecast.Domain.SharedRoot
{
    public class EntityBaseCompose
    {
        public int IdCelula { get; set; }
        public int IdCliente { get; set; }
        public int IdServico { get; set; }
        public int NrAno { get; set; }
        public string Usuario { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
