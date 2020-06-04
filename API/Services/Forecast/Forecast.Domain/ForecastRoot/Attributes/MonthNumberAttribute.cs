using System;
using System.Collections.Generic;
using System.Text;

namespace Forecast.Domain.ForecastRoot.Attributes
{
    public class MonthNumber : Attribute
    {
        public int Number { get; set; }

        public MonthNumber(int number)
        {
            this.Number = number;
        }
    }
}
