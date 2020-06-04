using Forecast.Domain.ForecastRoot;
using Forecast.Domain.ForecastRoot.Dto;
using System.Collections.Generic;
using Utils.Base;

namespace Forecast.Domain.ForecastRoot.Service.Interfaces
{
    public interface IForecastService
    {        
        void Adicionar(ForecastET forecast);
        void Atualizar(ForecastET forecast);        
        List<ForecastET> BuscarTodos();
        ForecastET BuscarPorId(int id);
        FiltroGenericoDtoBase<ForecastDto> Filtrar(FiltroGenericoDtoBase<ForecastDto> filtro);
        List<int> BuscarTodosAnos();
        ForecastET BuscarPorIdComposto(int idCelula, int idCliente, int idServico, int ano);
        void RealizarMigracao();
        void RealizarMigracaoBi();
        ForecastET BuscarPorIdComIncludes(int idCelula, int idCliente, int idServico, int nrAno);
        List<ComboClienteServicoDto> ObterServicoPorIdCelulaIdClienteEAcesso(int idCelula, int idCliente);
        ForecastET VerificarSeRegistroExiste(ForecastET forecast);
        int ObterQuantidadeDiasUteisAposViradaMes();
    }
}
