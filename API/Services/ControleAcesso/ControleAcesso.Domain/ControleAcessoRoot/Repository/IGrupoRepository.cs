using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using Utils.Base;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface IGrupoRepository : IBaseRepository<Grupo>
    {
        FiltroGenericoDtoBase<GrupoDto> FiltrarGrupos(FiltroGenericoDtoBase<GrupoDto> filtro);
        bool ValidarExisteGrupo(string descricao);
        bool ExisteGrupoComCelulasInativas(int idGrupo);
    }
}
