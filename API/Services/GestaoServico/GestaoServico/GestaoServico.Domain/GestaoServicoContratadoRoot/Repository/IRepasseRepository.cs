using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Repository
{
    public interface IRepasseRepository : IBaseRepository<Repasse>
    {
        FiltroGenericoDtoBase<GridRepasseDto> Filtrar(FiltroGenericoDtoBase<GridRepasseDto> filtro);
        FiltroAprovarRepasseDto FiltrarAprovar(FiltroAprovarRepasseDto filtro);
        Repasse ObterRepasePorIdPorData(int id, DateTime data);

        List<Repasse> ObterRepassesPorIdRelacionados(int id);
        void UpdateComposte(Repasse repasse);
        bool VerificarExistenciaRepasseFuturo(int id);
        Repasse ObterRepasseComDescricaoParcela(int id, DateTime data);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorServicoContratadoDestino(int idServicoContratado);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaDestino(int idCelula);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaClienteDestino(int idCelula, int idCliente);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorServicoContratadoOrigem(int idServicoContratado);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaOrigem(int idCelula);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulaClienteOrigem(int idCelula, int idCliente);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulasRelatorioDiretoriaDestino(List<int> idsCelula);
        List<RepasseRelatorioRentabilidadeModel> ObterIdsRepassePorCelulasRelatorioDiretoriaOrigem(List<int> idsCelula);
    }
}
