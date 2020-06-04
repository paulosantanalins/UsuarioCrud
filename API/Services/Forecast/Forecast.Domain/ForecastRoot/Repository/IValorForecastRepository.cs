using Forecast.Domain.ForecastRoot.Dto;
using System.Collections.Generic;
using Utils.Base;

namespace Forecast.Domain.ForecastRoot.Repository
{
    public interface IValorForecastRepository
    {   
        void Adicionar(ValorForecast entity);
        void Update(ValorForecast forecast);
        ICollection<ValorForecast> BuscarTodos();
     
    }
}
