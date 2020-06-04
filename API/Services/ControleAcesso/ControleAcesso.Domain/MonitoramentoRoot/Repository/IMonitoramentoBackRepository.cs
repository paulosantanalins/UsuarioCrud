using ControleAcesso.Domain.Interfaces;
using ControleAcesso.Domain.MonitoramentoRoot.DTO;
using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using Utils;

namespace ControleAcesso.Domain.MonitoramentoRoot.Repository
{
    public interface IMonitoramentoBackRepository : IBaseRepository<MonitoramentoBack>
    {
        FiltroGenericoDto<MonitoramentoBackDto> FiltrarBackend(FiltroGenericoDto<MonitoramentoBackDto> filtro);
    }
}
