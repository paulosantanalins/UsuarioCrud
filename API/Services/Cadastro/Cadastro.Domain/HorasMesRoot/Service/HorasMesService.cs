using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.HorasMesRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Utils.Connections;

namespace Cadastro.Domain.HorasMesRoot.Service
{
    public class HorasMesService : IHorasMesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IHorasMesRepository _horasMesRepository;
        private readonly IPeriodoDiaPagamentoRepository _periodoDiaPagamentoRepository;

        public HorasMesService(
            IUnitOfWork unitOfWork,
            IHorasMesRepository horasMesRepository,
            IPeriodoDiaPagamentoRepository periodoDiaPagamentoRepository,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
            _horasMesRepository = horasMesRepository;
            _periodoDiaPagamentoRepository = periodoDiaPagamentoRepository;
        }

        public IEnumerable<HorasMes> BuscarPeriodos()
        {
            var periodo = _horasMesRepository.BuscarPeriodos();
            return periodo;
        }

        public HorasMes BuscarPorId(int id)
        {
            var periodo = _horasMesRepository.BuscarPorId(id);
            return periodo;
        }

        public void AtualizarHorasMes(HorasMes horasMes)
        {
            _horasMesRepository.Update(horasMes);
            _periodoDiaPagamentoRepository.UpdateList(horasMes.PeriodosDiaPagamento, false);
            _unitOfWork.Commit();
        }

        public void SalvarHorasMes(HorasMes horasMes)
        {            
            _horasMesRepository.Adicionar(horasMes);
            _unitOfWork.Commit();
        }

        public void Inativar(int id)
        {
            var model = _horasMesRepository.BuscarPorId(id);

            if (model != null)
            {
                model.Ativo = !model.Ativo;
                _horasMesRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

    }
}
