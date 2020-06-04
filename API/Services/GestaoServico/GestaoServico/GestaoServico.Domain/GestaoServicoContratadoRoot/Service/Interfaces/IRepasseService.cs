using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces
{
    public interface IRepasseService
    {
        FiltroGenericoDtoBase<GridRepasseDto> Filtrar(FiltroGenericoDtoBase<GridRepasseDto> filtro);
        FiltroAprovarRepasseDto FiltrarAprovar(FiltroAprovarRepasseDto filtro);
        Repasse ObterRepassePorId(int id);
        Repasse ObterRepassePorIdPorData(int id, DateTime dataRepasse);
        void PersistirRepasse(Repasse repasse, int vezesRepetidas);
        void CancelarRepasse(int id);
        bool VerificarExistenciaParcelasFuturas(int id);
        void CancelarRepasseUnico(int id);
        void AtualizarRepasse(Repasse repasse);
        void AtualizarRepassesFuturos(Repasse repasse);
        Repasse ObterRepassePorIdPorDataComDescricaoParcelada(int id, DateTime dataRepasse);
        void NegarRepasse(int id, string motivo);
        void AprovarRepasse(int id);
        void ResetarRepasse(int id);
        List<GridRepasseAprovarDto> AprovarBlocoRepasse(List<GridRepasseAprovarDto> repasses);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorServicoContratadoDestino(int idServicoContratado);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaDestino(int idCelula);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaClienteDestino(int idCelula, int idCliente);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorServicoContratadoOrigem(int idServicoContratado);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaOrigem(int idCelula);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulaClienteOrigem(int idCelula, int idCliente);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulasDestinoRelatorioDiretoria(List<int> idsCelula);
        List<RepasseRelatorioRentabilidadeModel> ObterRepassesPorCelulasOrigemRelatorioDiretoria(List<int> idsCelula);
    }
}
