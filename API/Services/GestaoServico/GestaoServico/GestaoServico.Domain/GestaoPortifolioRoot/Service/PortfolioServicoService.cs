using System.Collections.Generic;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPortifolioRoot.Validators;
using GestaoServico.Domain.Interfaces;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service
{
    public class PortfolioServicoService : IPortfolioServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationHandler _notificationHandler;
        private readonly Variables _variables;
        private readonly IPortfolioServicoRepository _portfolioServicoRespository;

        public PortfolioServicoService(
           IPortfolioServicoRepository portfolioServicoRespository,
           IUnitOfWork unitOfWork,
           NotificationHandler notification,
           Variables variables)
        {
            _portfolioServicoRespository = portfolioServicoRespository;
            _unitOfWork = unitOfWork;
            _notificationHandler = notification;
            _variables = variables;
        }

        public PortfolioServico BuscarPorId(int id)
        {
            var result = _portfolioServicoRespository.BuscarPorId(id);
            return result;
        }


        public IEnumerable<PortfolioServico> BuscarTodas()
        {
            throw new System.NotImplementedException();
        }

        public FiltroGenericoDto<PortfolioServicoDto> Filtrar(FiltroGenericoDto<PortfolioServicoDto> filtro)
        {
            var result = _portfolioServicoRespository.Filtrar(filtro);
            return result;
        }

        public void Inativar(int id)
        {
            var model = _portfolioServicoRespository.BuscarPorId(id);

            if (model != null)
            {
                model.FlStatus = !model.FlStatus;
                _portfolioServicoRespository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public IEnumerable<PortfolioServico> ObterTodos()
        {
            return _portfolioServicoRespository.BuscarTodos();
        }

        public void Persistir(PortfolioServico portfolioServico)
        {
            var mensagens = new PortfolioServicoValidator().Validate(portfolioServico).Errors;
            if (mensagens.Count > 0)
            {
                foreach (var mensagem in mensagens)
                {
                    _notificationHandler.AddMensagem(mensagem.PropertyName, mensagem.ErrorMessage);
                }
                return;
            }

            if ((_portfolioServicoRespository.Validar(portfolioServico)))
            {
                _notificationHandler.AddMensagem("DUPLICIDADE", "PERSISTIR_PORTFOLIO_SERVICO_DUPLICIDADE");                                    
                return;
            }

            if (portfolioServico.Id == 0)
            {
                _portfolioServicoRespository.Adicionar(portfolioServico);
            }
            else
            {
                _portfolioServicoRespository.Update(portfolioServico);
            }
            _unitOfWork.Commit();
        }

        public bool ValidarInexistencia(int id)
        {
            var result = _portfolioServicoRespository.ValidarInativacao(id);
            return result;
        }

        public IEnumerable<PortfolioServico> ObterTodosAtivos()
        {
            return _portfolioServicoRespository.Buscar(x => x.FlStatus);
        }
    }
}
