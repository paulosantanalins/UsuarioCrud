using ControleAcesso.Domain.MonitoramentoRoot.DTO;
using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using ControleAcesso.Domain.MonitoramentoRoot.Repository;
using ControleAcesso.Domain.MonitoramentoRoot.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace ControleAcesso.Domain.MonitoramentoRoot.Service
{
    public class MonitoramentoBackService : IMonitoramentoBackService
    {
        private readonly IMonitoramentoBackRepository _monitoramentoBackRepository;

        public MonitoramentoBackService(
            IMonitoramentoBackRepository monitoramentoBackRepository)
        {
            _monitoramentoBackRepository = monitoramentoBackRepository;
        }

        public FiltroGenericoDto<MonitoramentoBackDto> FiltrarBackend(FiltroGenericoDto<MonitoramentoBackDto> filtro)
        {
            var result = _monitoramentoBackRepository.FiltrarBackend(filtro);
            return result;
        }

        public List<MonitoramentoBack> PopularComboOrigem()
        {
            var result = _monitoramentoBackRepository.BuscarTodos();
            return result.ToList();
        }
    }
}
