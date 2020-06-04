using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Repository
{
    public interface IServicoContratadoRepository : IBaseRepository<ServicoContratado>
    {
        FiltroGenericoDto<ServicoContratadoDto> Filtrar(FiltroGenericoDto<ServicoContratadoDto> filtro);
        int Validar(ServicoContratado servicoContratado);
        ServicoContratado BuscarComInclude(int id);
        List<MultiselectDto> PreencherComboServicoContratadoPorCelulaCliente(int idCelula, int idCliente);
        List<MultiselectDto> ObterServicoContratadoPorCliente(int idCliente);
        int? VerificarServicoContratadoComercialUnicoPorContrato(int idContrato);
        int? ObterCelulaComercialVigenteContrato(int idContrato);
        List<MultiselectDto> ObterServicoContratadoPorCelula(int idCelula);

        List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaRelatorioRentabilidade(int idCelula);
        List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaClienteRelatorioRentabilidade(int idCelula, int idCliente);
        List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulasRelatorioDiretoria(List<int> idsCelula);
        FiltroGenericoDto<ServicoContratado> FiltrarAccordions(FiltroGenericoDto<ServicoContratado> filtro);
        ServicoContratado ObterServicoComercialVigenteContrato(int idContrato);

    }
}
