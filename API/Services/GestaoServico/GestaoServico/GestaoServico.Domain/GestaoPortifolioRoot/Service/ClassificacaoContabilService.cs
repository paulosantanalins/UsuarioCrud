using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPortifolioRoot.Validators;
using GestaoServico.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service
{
    public class ClassificacaoContabilService : IClassificacaoContabilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationHandler _notificationHandler;
        //private readonly Variables _variables;
        private readonly IClassificacaoContabilRepository _classificacaoContabilRepository;

        public ClassificacaoContabilService(
            IClassificacaoContabilRepository classificacaoContabilRepository,
            IUnitOfWork unitOfWork,
            NotificationHandler notification/*,
            Variables variables*/)
        {
            _classificacaoContabilRepository = classificacaoContabilRepository;
            _unitOfWork = unitOfWork;
            _notificationHandler = notification;
          //  _variables = variables;
        }

        public void Persistir(ClassificacaoContabil classificacaoContabil)
        {
            var mensagens = new ClassificacaoContabilValidator().Validate(classificacaoContabil).Errors;
            if (mensagens.Count > 0)
            {
                foreach (var mensagem in mensagens)
                {
                    _notificationHandler.AddMensagem(mensagem.PropertyName, mensagem.ErrorMessage);
                }
                return;
            }

            if (_classificacaoContabilRepository.Validar(classificacaoContabil))
            {
                _notificationHandler.AddMensagem("Sigla", "PERSISTIR_CLASSIFICACAO_CONTABIL_SIGLA_IGUAL");
                return;
            }

            if (_classificacaoContabilRepository.ValidarDescricao(classificacaoContabil))
            {
                _notificationHandler.AddMensagem("Sigla", "PERSISTIR_CLASSIFICACAO_CONTABIL_DESCRICAO_IGUAL");
                return;
            }

            if (classificacaoContabil.Id == 0)
            {
                _classificacaoContabilRepository.Adicionar(classificacaoContabil);
            }
            else
            {
                _classificacaoContabilRepository.Update(classificacaoContabil);
            }

            _unitOfWork.Commit();
        }

        public void Inativar(int id)
        {
            var model = _classificacaoContabilRepository.BuscarPorId(id);

            if (model != null)
            {
                model.FlStatus = !model.FlStatus;
                _classificacaoContabilRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public FiltroGenericoDto<ClassificacaoContabilDto> Filtrar(FiltroGenericoDto<ClassificacaoContabilDto> filtro)
        {
            var result = _classificacaoContabilRepository.Filtrar(filtro);
            return result;
        }

        public ClassificacaoContabil BuscarPorId(int id)
        {
            var result = _classificacaoContabilRepository.BuscarPorId(id);
            return result;
        }

        public ICollection<ClassificacaoContabil> obterClassificacoesPorCategoria(int idCategoria)
        {
            var result = _classificacaoContabilRepository.BuscarClassificacoes(idCategoria);
            return result;
        }

        public bool ValidarInativacaoPorId(int id)
        {
            var result = _classificacaoContabilRepository.ValidarInexistencia(id);
            return result;
        }
    }

}
