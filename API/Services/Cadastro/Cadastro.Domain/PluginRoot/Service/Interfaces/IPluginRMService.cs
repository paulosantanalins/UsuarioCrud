using Cadastro.Domain.PluginRoot.Dto;
using System.Collections.Generic;
using System.Data;

namespace Cadastro.Domain.PluginRoot.Service.Interfaces
{
    public interface IPluginRMService
    {
        void SolicitarPagamentoRM(int id);
        void AtualizarSituacaoApartirDoRm();
        void EnviarPrestadorRM(int idPrestador, string operacao, bool atualizaEmpresa);
        void EnviarEmpresaRM(int idPrestador, IDbConnection dbConnection, string idRepresentanteRM);
        IEnumerable<LogErroRMDto> ObterLogErroRm(int idInt);
    }
}
