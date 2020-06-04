using Forecast.Domain.ForecastRoot.Dto;
using System.Collections.Generic;
using Utils.Base;

namespace Forecast.Domain.ForecastRoot.Repository
{
    public interface IForecastRepository
    {
        FiltroGenericoDtoBase<ForecastDto> Filtrar(FiltroGenericoDtoBase<ForecastDto> filtro, List<int> idClientes, List<int> idServicos);
        ForecastET Buscar(int idCelula, int idCliente, int idServico, int ano);
        ForecastET BuscarPorIdComIncludes(int idCelula, int idCliente, int idServico, int nrAno);
        void Adicionar(ForecastET entity);
        void Update(ForecastET forecast);
        ICollection<ForecastET> BuscarTodos();
        ForecastET BuscarPorId(int id);
        void AdicionarRange(List<ForecastET> entities);
        List<ForecastET> BuscarTodosPorComIncludes();
        bool? DefinirAlertaAniversarioForecast(ForecastDto forecast);
    }
}
