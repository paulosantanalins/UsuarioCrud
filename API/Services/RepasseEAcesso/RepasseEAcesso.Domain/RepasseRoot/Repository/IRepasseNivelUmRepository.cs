using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.Repository;
using System;
using System.Collections.Generic;
using Utils.Base;

namespace RepasseEAcesso.Domain.RepasseRoot.Repository
{
    public interface IRepasseNivelUmRepository : IBaseRepository<RepasseNivelUm>
    {
        FiltroGenericoDtoBase<AprovarRepasseDto> Filtrar(FiltroGenericoDtoBase<AprovarRepasseDto> filtro);
        Boolean ValidarUsuarioResponsavelPelaCelula(int idCelulaDestino);
        FiltroRepasseNivelUmDto<AprovarRepasseDto> Filtrar(FiltroRepasseNivelUmDto<AprovarRepasseDto> filtro);
        FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> Filtrar(FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> filtro);
        List<RepasseNivelUm> BuscarComInclude();
        List<RepasseNivelUm> BuscarTodosFilhos(int idRepasse);
        List<RepasseNivelUm> BuscarTodosFilhosLegado(int idRepasseEacesso);
        RepasseNivelUm BuscarComIncludeId(int id);
        void DetachEntity(RepasseNivelUm repasse);
        IEnumerable<string> ObterLoginsComFuncionalidadeAprovarNivelDois();
    }
}
