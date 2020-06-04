using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using System.Collections.Generic;
using Utils.Base;

namespace RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces
{
    public interface IRepasseNivelUmService
    {
        FiltroGenericoDtoBase<AprovarRepasseDto> Filtrar(FiltroGenericoDtoBase<AprovarRepasseDto> filtro);
        FiltroRepasseNivelUmDto<AprovarRepasseDto> FiltrarRepassesNivelUm(FiltroRepasseNivelUmDto<AprovarRepasseDto> filtro);
        FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> FiltrarRepassesNivelDois(FiltroRepasseNivelDoisDto<AprovarRepasseNivelDoisDto> filtro);
        List<RepasseNivelUm> BuscarTodos();
        RepasseNivelUm BuscarPorId(int id);
        void AprovarRepasse(AprovarRepasseDto aprovarRepasseDto);
        void AprovarRepasseNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto);
        void NegarRepasse(AprovarRepasseDto aprovarRepasseDto);
        void NegarRepasseNivelDois(AprovarRepasseNivelDoisDto aprovarRepasseDto);
        void Persistir(RepasseNivelUm repasseStfCorp, Repasse repasseEacesso);
        RepasseNivelUm BuscarComIncludeId(int id);        
    }
}