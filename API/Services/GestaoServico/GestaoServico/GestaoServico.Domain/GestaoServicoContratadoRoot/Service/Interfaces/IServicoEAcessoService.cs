using System.Collections.Generic;
using System.Data;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces
{
    public interface IServicoEAcessoService
    {
        List<ComboClienteServicoDto> ObterServicoAtivoPorIdCelulaIdClienteEAcesso(int idCelula, int idCliente);
        List<ComboClienteServicoDto> ObterTodosServicoPorIdCelulaIdClienteEAcesso(int idCelula, int idCliente);
        ServicoContratado BuscarServicoEAcesso(IDbConnection dbConnection, int idServico);
        List<int> ObterServicosForaDiretoria(IDbConnection dbConnection, List<int> idsServicosFaltantes,
            List<int> celulasDiretoria);
        List<ServicoMigracaoDTO> ObterServicosCompletos(List<int> idsServicos);
        List<int> BuscarIdServicosPorNomeServico(string nome);
        List<ComboDefaultDto> ObterServicosPorIdsEacesso(string servicos);
        ComboClienteServicoDto ObterServicoPorId(int idServico);
    }
}
