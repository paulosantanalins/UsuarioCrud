using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.ControleAcessoRoot.Validators;
using ControleAcesso.Domain.Core.Notifications;
using ControleAcesso.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service
{
    public class PerfilService : IPerfilService
    {
        protected readonly NotificationHandler _notificationHandler;
        protected readonly IPerfilRepository _perfilRepository;
        protected readonly IFuncionalidadeRepository _funcionalidadeRepository;
        protected readonly IVinculoPerfilFuncionalidadeRepository _vinculoPerfilFuncionalidadeRepository;
        protected readonly IVariablesToken _variables;
        protected readonly IUnitOfWork _unitOfWork;

        public PerfilService(IPerfilRepository perfilRepository,
                             IVariablesToken variables,
                             IUnitOfWork unitOfWork,
                             IFuncionalidadeRepository funcionalidadeRepository,
                             IVinculoPerfilFuncionalidadeRepository vinculoPerfilFuncionalidadeRepository,
                             NotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
            _perfilRepository = perfilRepository;
            _variables = variables;
            _unitOfWork = unitOfWork;
            _funcionalidadeRepository = funcionalidadeRepository;
            _vinculoPerfilFuncionalidadeRepository = vinculoPerfilFuncionalidadeRepository;
        }


        public void PersistirPerfil(Perfil perfil)
        {
            var mensagens = new PerfilValidator().Validate(perfil).Errors;
            if (mensagens.Count > 0)
            {
                foreach (var mensagem in mensagens)
                {
                    _notificationHandler.AddMensagem(mensagem.PropertyName, mensagem.ErrorMessage);
                }
                return;
            }
            if (_perfilRepository.Validar(perfil))
            {
                _notificationHandler.AddMensagem("Sigla", "PERSISTIR_PERFIL_DUPLICIDADE_NOME");
                return;
            }
            perfil.DataAlteracao = DateTime.Now;
            perfil.Usuario = _variables.UserName;
            if (perfil.Id == 0)
            {
                _perfilRepository.Adicionar(perfil);
            }
            else
            {
                AtualizarPerfil(perfil);
            }
            _unitOfWork.Commit();
        }


        public void AtualizarPerfil(Perfil perfil)
        {
            var perfilDb = _perfilRepository.ObterPerfilComVinculo(perfil.Id);
            var funcionalidades = _funcionalidadeRepository.BuscarTodos();
            var funcionalidadesRemovidas = perfilDb.VinculoPerfilFuncionalidades.Where(x => !perfil.VinculoPerfilFuncionalidades.Any(y => y.IdFuncionalidade == x.IdFuncionalidade));
            var funcionalidadesMantidas = perfil.VinculoPerfilFuncionalidades.Where(x => perfilDb.VinculoPerfilFuncionalidades.Any(y => y.IdFuncionalidade == x.IdFuncionalidade));
            var funcionalidadesAdicionadas = perfil.VinculoPerfilFuncionalidades.Where(x => !perfilDb.VinculoPerfilFuncionalidades.Any(y => y.Id == x.Id) && !funcionalidadesMantidas.Any(y => y.IdFuncionalidade == x.IdFuncionalidade));
            if (funcionalidadesRemovidas.Any())
            {
                _vinculoPerfilFuncionalidadeRepository.RemoverVinculos(funcionalidadesRemovidas.ToList());
            }
            if (funcionalidadesAdicionadas.Any())
            {
                foreach (var item in funcionalidadesAdicionadas.ToList())
                {
                    perfilDb.VinculoPerfilFuncionalidades.Add(item);

                }
            }
            perfilDb.FlAtivo = perfil.FlAtivo;
            perfilDb.NmModulo = perfil.NmModulo;
            perfilDb.NmPerfil = perfil.NmPerfil;

            _perfilRepository.Update(perfilDb);
        }


        public void InativarPerfil(int id)
        {
            var perfil = _perfilRepository.BuscarPorId(id);
            perfil.DataAlteracao = DateTime.Now;
            perfil.Usuario = _variables.UserName;
            perfil.FlAtivo = !perfil.FlAtivo;
            _unitOfWork.Commit();
        }

        public bool VerificarExistenciaUsuariosComPerfil(int id)
        {
            var result = _perfilRepository.ObterPerfilComUsuarios(id);
            return result != null;
        }

        public FiltroGenericoDto<PerfilDto> FiltrarPerfil(FiltroGenericoDto<PerfilDto> filtro)
        {
            var result = _perfilRepository.FiltrarPerfil(filtro);
            return result;
        }

        public Perfil BuscarPorId(int id)
        {
            var result = _perfilRepository.ObterPerfilComVinculo(id);

            return result;
        }

        public List<Perfil> BuscarTodosPerfis()
        {
            var result = _perfilRepository.BuscarTodos();
            return result.ToList();
        }

        public List<Perfil> BuscarTodosPerfisAtivos()
        {
            var result = _perfilRepository.Buscar(x => x.FlAtivo).OrderBy(x => x.NmPerfil);
            return result.ToList();
        }
                
        public List<Perfil> BuscarPerfisPorFuncionalidades(int[] funcionalidadesIds)
        {
            //var result = _perfilRepository.Buscar(x => funcionalidades.Contains(x.VinculoPerfilFuncionalidades.)).ToList();
            var result = _perfilRepository.BuscarPerfisPorFuncionalidades(funcionalidadesIds);
            return result;
        }
    }
}
