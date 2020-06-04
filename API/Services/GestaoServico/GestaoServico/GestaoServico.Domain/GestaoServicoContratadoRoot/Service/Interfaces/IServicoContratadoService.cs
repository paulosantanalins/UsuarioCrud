using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using System.Collections.Generic;
using Utils;
using Utils.Base;
using Utils.Relatorios.Models;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces
{
    public interface IServicoContratadoService
    {
        int Persistir(ServicoContratado servicoContratado, int idCelula, decimal? vlMarkup);
        void PersistirServicoMigrado(ServicoMigracaoDTO servicoMigradoDTO);
        FiltroGenericoDto<ServicoContratadoDto> Filtrar(FiltroGenericoDto<ServicoContratadoDto> filtroDto);
        ServicoContratado BuscarPorId(int id);
        List<ServicoContratado> PreencherComboServicoContratado();
        List<MultiselectDto> PreencherComboServicoContratadoPorCelulaCliente(int idCelula, int idCliente);
        List<MultiselectDto> ObterServicoContratadoPorCliente(int idCliente);
        List<MultiselectDto> ObterServicoContratadoPorCelula(int idCelula);
        List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorClienteRelatorioRentabilidade(int idCelula, int idCliente);
        List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaRelatorioRentabilidade(int idCelula);
        int PersistirServicoEacesso(ServicoContratado servico, int idServicoEacesso, string nomeServico, int idCliente, decimal markup, string idContrato, string siglaTipoServico, string descEscopo, string siglaSubServico = null);
        List<ServicoContratadoRelatorioRentabilidadeModel> ObterServicoContratadoPorCelulaRelatorioDiretoria(List<int> idsCelula);
        void MigrarServicoEacesso(ServicoContratado servico);
        FiltroGenericoDto<ServicoContratado> FiltrarAccordions(FiltroGenericoDto<ServicoContratado> filtroDto);
        ServicoContratado ObterServicoComercialAtivoPorContrato(int idContrato);
        void Criar(ServicoContratado servicoContratado);
    }
}
