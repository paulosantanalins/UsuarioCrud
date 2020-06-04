using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPortifolioRoot.DTO;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Utils.Base;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service
{
    public class EscopoServicoService : IEscopoServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationHandler _notificationHandler;
        private readonly Variables _variables;
        private readonly IEscopoServicoRepository _escopoServicoRepository;

        public EscopoServicoService(
            IUnitOfWork unitOfWork,
            NotificationHandler notificationHandler,
            Variables variables,            
            IEscopoServicoRepository escopoServicoRepository)
        {
            _unitOfWork = unitOfWork;
            _notificationHandler = notificationHandler;
            _variables = variables;
            _escopoServicoRepository = escopoServicoRepository;
        }


        public void PersistirEscopoServico(EscopoServico escopoServico)
        {
            ValidarEscopoValido(escopoServico);
            if (!_notificationHandler.Mensagens.Any())
            {
                if (escopoServico.Id != 0)
                {
                    _escopoServicoRepository.Update(escopoServico);
                }
                else
                {
                    _escopoServicoRepository.Adicionar(escopoServico);
                }
                _unitOfWork.Commit();
            }
        }

        public void ValidarEscopoValido(EscopoServico escopoServico)
        {
            if (_escopoServicoRepository.VerificarExistenciaEscopoServicoIgual(escopoServico))
            {
                _notificationHandler.AddMensagem("", "ESCOPO_JA_EXISTENTE");
            }
        }


        public FiltroGenericoDto<GridEscopoDTO> Filtrar(FiltroGenericoDto<GridEscopoDTO> filtro)
        {
            var result = _escopoServicoRepository.Filtrar(filtro);
            return result;
        }

        public EscopoServico BuscarPorId(int idEscopo)
        {
            var result = _escopoServicoRepository.BuscarPorId(idEscopo);
            return result;
        }

        public void AlterarStatus(int idEscopo)
        {
            var escopoDB = BuscarPorId(idEscopo);
            escopoDB.FlAtivo = !escopoDB.FlAtivo;
            _escopoServicoRepository.Update(escopoDB);
            _unitOfWork.Commit();
        }

        public IEnumerable<MultiselectDto> BuscarAtivos()
        {
            var result = _escopoServicoRepository.ObterAtivos();
            return result;
        }

        public bool VerificarExclusaoValida(int id)
        {
            return _escopoServicoRepository.VerificarExclusaoValida(id);
        }

        public IEnumerable<MultiselectDto> BuscarAtivosPorPortfolio(int idPortfolio)
        {
            var result = _escopoServicoRepository.ObterAtivosPorPortfolio(idPortfolio);
            return result;
        }
    }
}
