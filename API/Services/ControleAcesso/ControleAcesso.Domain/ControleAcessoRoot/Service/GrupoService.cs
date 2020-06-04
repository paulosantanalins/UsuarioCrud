using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using Utils.Base;
using Utils.Connections;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service
{
    public class GrupoService : IGrupoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IGrupoRepository _grupoRepository;
        public GrupoService(
            IUnitOfWork unitOfWork,
            IGrupoRepository grupoRepository,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
            _grupoRepository = grupoRepository;
        }        

        public Grupo BuscarPorId(int id)
        {
            var grupo = _grupoRepository.BuscarPorId(id);
            return grupo;
        }

        public void AtualizarGrupo(Grupo grupo)
        {
            var grupoBd = _grupoRepository.BuscarPorId(grupo.Id);
            grupoBd.DescGrupo = grupo.DescGrupo;
            _grupoRepository.Update(grupoBd);
            _unitOfWork.Commit();
        }

        public void SalvarGrupo(Grupo grupo)
        {
            _grupoRepository.Adicionar(grupo);
            _unitOfWork.Commit();
        }

        public void Inativar(int id)
        {
            var model = _grupoRepository.BuscarPorId(id);

            if (model != null)
            {
                model.Ativo = false;
                _grupoRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public void Reativar(int id)
        {
            var model = _grupoRepository.BuscarPorId(id);

            if (model != null)
            {
                model.Ativo = true;
                _grupoRepository.Update(model);
                _unitOfWork.Commit();
            }
        }

        public FiltroGenericoDtoBase<GrupoDto> Filtrar(FiltroGenericoDtoBase<GrupoDto> filtro)
        {
            return _grupoRepository.FiltrarGrupos(filtro); 
        }

        public IEnumerable<Grupo> BuscarTodos()
        {
            var grupos = _grupoRepository.BuscarTodos();
            return grupos;
        }

        public IEnumerable<Grupo> BuscarTodosAtivos()
        {
            var grupos = _grupoRepository.BuscarTodos().Where(x => x.Ativo);
            return grupos;
        }

        public bool ValidarExisteGrupo(string descricao)
        {
            return _grupoRepository.ValidarExisteGrupo(descricao);
        }

        public bool ExisteGrupoComCelulasInativas(int idGrupo)
        {
            return _grupoRepository.ExisteGrupoComCelulasInativas(idGrupo);
        }
    }
}
