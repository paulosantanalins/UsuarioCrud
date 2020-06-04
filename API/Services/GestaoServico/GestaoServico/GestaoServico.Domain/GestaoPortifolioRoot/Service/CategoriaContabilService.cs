using System.Collections.Generic;
using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPortifolioRoot.Validators;
using GestaoServico.Domain.Interfaces;
using Utils;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service
{
    public class CategoriaContabilService : ICategoriaContabilService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationHandler _notificationHandler;
        private readonly Utils.Variables _variables;
        private readonly ICategoriaContabilRepository _categoriaContabilRepository;

        public CategoriaContabilService(
            ICategoriaContabilRepository categoriaContabilRepository,
            IUnitOfWork unitOfWork,
            NotificationHandler notification ,
           Utils.Variables variables)
        {
            _categoriaContabilRepository = categoriaContabilRepository;
            _unitOfWork = unitOfWork;
            _notificationHandler = notification;
            _variables = variables;
        }

        public CategoriaContabil BuscarPorId(int id)
        {
            var result = _categoriaContabilRepository.BuscarPorId(id);
            return result;
        }

        public IEnumerable<CategoriaContabil> BuscarTodas()
        {
            var result = _categoriaContabilRepository.BuscarTodos();
            return result;
        }

        public IEnumerable<CategoriaContabil> BuscarTodasAtivas()
        {
            var result = _categoriaContabilRepository.ObterAtivos();
            return result;
        }

        public FiltroGenericoDto<CategoriaContabil> Filtrar(FiltroGenericoDto<CategoriaContabil> filtro)
        {
            var result = _categoriaContabilRepository.Filtrar(filtro);
            return result;
        }

        public void Inativar(int id)
        {
            var model = _categoriaContabilRepository.BuscarPorId(id);

            if (model != null)
            {
                model.FlStatus = !model.FlStatus;
                _categoriaContabilRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public void Persistir(CategoriaContabil categoria)
        {
            var mensagens = new CategoriaValidator().Validate(categoria).Errors;
            if (mensagens.Count > 0)
            {
                foreach (var mensagem in mensagens)
                {
                    _notificationHandler.AddMensagem(mensagem.PropertyName, mensagem.ErrorMessage);
                }
                return;
            }

            if ((_categoriaContabilRepository.Validar(categoria)))
            {
                _notificationHandler.AddMensagem("Sigla", "PERSISTIR_CATEGORIA_DUPLICIDADE");
                return;
            }

            if (categoria.Id == 0)
            {
                _categoriaContabilRepository.Adicionar(categoria);
            }
            else
            {
                _categoriaContabilRepository.Update(categoria);
            }
            _unitOfWork.Commit();
        }

        public bool ValidarInativacaoPorId(int id)
        {
            var result = _categoriaContabilRepository.ValidarInexistencia(id);
            return result;
        }
    }
}
