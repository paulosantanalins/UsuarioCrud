using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using System.Collections.Generic;
using Utils.Base;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces
{
    public interface IGrupoService
    {        
        Grupo BuscarPorId(int id);
        void AtualizarGrupo(Grupo grupo);
        void SalvarGrupo(Grupo grupo);
        void Inativar(int id);
        void Reativar(int id);
        FiltroGenericoDtoBase<GrupoDto> Filtrar(FiltroGenericoDtoBase<GrupoDto> filtro);
        IEnumerable<Grupo> BuscarTodos();
        bool ValidarExisteGrupo(string descricao);
        bool ExisteGrupoComCelulasInativas(int idGrupo);
        IEnumerable<Grupo> BuscarTodosAtivos();
    }
}
