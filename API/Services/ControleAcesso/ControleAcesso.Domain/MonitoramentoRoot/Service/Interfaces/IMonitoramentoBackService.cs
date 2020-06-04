using ControleAcesso.Domain.MonitoramentoRoot.DTO;
using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using System.Collections.Generic;
using Utils;

namespace ControleAcesso.Domain.MonitoramentoRoot.Service.Interfaces
{
    public interface IMonitoramentoBackService
    {
        FiltroGenericoDto<MonitoramentoBackDto> FiltrarBackend(FiltroGenericoDto<MonitoramentoBackDto> filtro);
        List<MonitoramentoBack> PopularComboOrigem();
    }
}
