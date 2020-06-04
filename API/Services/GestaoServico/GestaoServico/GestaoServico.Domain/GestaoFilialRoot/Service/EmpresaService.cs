using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoFilialRoot.Entity;
using GestaoServico.Domain.GestaoFilialRoot.Repository;
using GestaoServico.Domain.GestaoFilialRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace GestaoServico.Domain.GestaoFilialRoot.Service
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationHandler _notificationHandler;
        private readonly Variables _variables;
        private readonly IEmpresaRepository _empresaRepository;

        public EmpresaService(
           IEmpresaRepository empresaRepository,
           IUnitOfWork unitOfWork,
           NotificationHandler notification,
           Variables variables)
        {
            _empresaRepository = empresaRepository;
            _unitOfWork = unitOfWork;
            _notificationHandler = notification;
            _variables = variables;
        }

        public IEnumerable<Empresa> ObterTodos()
        {
            return _empresaRepository.BuscarTodos();
        }
    }
}
