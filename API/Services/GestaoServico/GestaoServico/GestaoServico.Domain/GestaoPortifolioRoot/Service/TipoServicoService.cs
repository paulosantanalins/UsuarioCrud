using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPortifolioRoot.Validators;
using GestaoServico.Domain.Interfaces;
using System.Collections.Generic;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service
{
    public class TipoServicoService : ITipoServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationHandler _notificationHandler;
        private readonly Utils.Variables _variables;
        private readonly ITipoServicoRepository _tipoServicoRepository;

        public TipoServicoService(
            ITipoServicoRepository tipoServicoRepository,
            IUnitOfWork unitOfWork,
            NotificationHandler notification,
            Utils.Variables variables)
        {
            _tipoServicoRepository = tipoServicoRepository;
            _unitOfWork = unitOfWork;
            _notificationHandler = notification;
            _variables = variables;
        }

        public TipoServico BuscarPorId(int id)
        {
            var result = _tipoServicoRepository.BuscarPorId(id);
            return result;
        }

        public FiltroGenericoDto<TipoServico> Filtrar(FiltroGenericoDto<TipoServico> filtro)
        {
            var result = _tipoServicoRepository.Filtrar(filtro);
            return result;
        }

        public void Inativar(int id)
        {
            var model = _tipoServicoRepository.BuscarPorId(id);

            if (model != null)
            {
                model.FlStatus = !model.FlStatus;
                _tipoServicoRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public void Persistir(TipoServico tipoServico)
        {
            var mensagens = new TipoServicoValidator().Validate(tipoServico).Errors;
            if (mensagens.Count > 0)
            {
                foreach (var mensagem in mensagens)
                {
                    _notificationHandler.AddMensagem(mensagem.PropertyName, mensagem.ErrorMessage);
                }
                return;
            }

            if (_tipoServicoRepository.Validar(tipoServico))
            {
                _notificationHandler.AddMensagem("Descrição", "PERSISTIR_TIPO_SERVICO_DUPLICIDADE");
                return;
            }

            if (tipoServico.Id == 0)
            {
                _tipoServicoRepository.Adicionar(tipoServico);
            }
            else
            {
                //var tipoServicoBD = _tipoServicoRepository.BuscarPorId(tipoServico.Id);
                //tipoServicoBD.DescTipoServico = tipoServico.DescTipoServico;
                //tipoServicoBD.FlStatus = tipoServico.FlStatus;
                _tipoServicoRepository.Update(tipoServico);
            }
            _unitOfWork.Commit();
        }

        public IEnumerable<TipoServico> BuscarTodos()
        {
            var result = _tipoServicoRepository.BuscarTodos();
            return result;
        }

        public bool VerificarExisteciaPorId(int id)
        {
            var result = _tipoServicoRepository.ValidarInativação(id);
            return result;
        }

        public IEnumerable<TipoServico> BuscarTodosAtivos()
        {
            var result = _tipoServicoRepository.Buscar(x => x.FlStatus);
            return result;
        }
    }
    
}
