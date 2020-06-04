using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Forecast.Domain.SharedRoot
{
    public class SharedEnuns
    {
        public enum StatusForecast
        {
            [Description("Reajuste Efetivado")]
            REAJUSTE_EFETIVADO = 0,
            [Description("Em negociação com cliente")]
            EM_NEGOCIACAO_COM_CLIENTE = 1, 
            [Description("Pendente de homologação sindical")]
            PENDENTE_DE_HOMOLOGACAO_SINDICAL = 2,
            [Description("Aguardando aniversário do contrato")]
            AGUARDANDO_ANIVERSARIO_DO_CONTRATO = 3
          
        }
    }
}
