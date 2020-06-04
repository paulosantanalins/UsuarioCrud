using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.DTO;
using Microsoft.Extensions.Options;
using Utils.Connections;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service
{
    public class ServicoEAcessoService : IServicoEAcessoService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ServicoEAcessoService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public List<ComboClienteServicoDto> ObterServicoAtivoPorIdCelulaIdClienteEAcesso(int idCelula, int idCliente)
        {
            List<ComboClienteServicoDto> servicos;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();                
                var query = "select distinct s.IdServico as id, s.Nome as descricao, s.SiglaTipoServico as Categoria" +
                            " from stfcorp.tblClientesServicos s, stfcorp.tblClientes c " +
                            "where s.IdCliente = " + idCliente + "and s.IdCelula = " + idCelula + " and s.dtInativacao is null and s.in_inativo is null ORDER BY descricao;";
                servicos = dbConnection.Query<ComboClienteServicoDto>(query).ToList();
                dbConnection.Close();
            }
            return servicos;
        }

        public List<ComboClienteServicoDto> ObterTodosServicoPorIdCelulaIdClienteEAcesso(int idCelula, int idCliente)
        {
            List<ComboClienteServicoDto> servicos;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select distinct s.IdServico as id, s.Nome as descricao, s.SiglaTipoServico as Categoria" +
                            " from stfcorp.tblClientesServicos s, stfcorp.tblClientes c " +
                            "where s.IdCliente = " + idCliente + "and s.IdCelula = " + idCelula + " ORDER BY descricao;";
                servicos = dbConnection.Query<ComboClienteServicoDto>(query).ToList();
                dbConnection.Close();
            }
            return servicos;
        }

        public ServicoContratado BuscarServicoEAcesso(IDbConnection dbConnection, int idServico)
        {
            var query = @"SELECT idServico, idCliente, idCelula, (nome + ' || ' + descricao) as descricaoservicocontratado, dtCriacao as DtInicial, dtInativacao as DtFinal
                            FROM [STFCORP].tblclientesservicos where idservico = " + idServico;
            var servico = dbConnection.Query<ServicoContratado>(query).FirstOrDefault();

            return servico;
        }

        public List<int> ObterServicosForaDiretoria(IDbConnection dbConnection, List<int> idsServicosFaltantes, List<int> celulasDiretoria)
        {
            var servicosConcatenados = string.Join(",", idsServicosFaltantes);
            var celulasConcatenadas = string.Join(",", celulasDiretoria);
            var query = "select idservico from stfcorp.tblclientesservicos where idservico in (" + servicosConcatenados + ") and idcelula not in (" + celulasConcatenadas + ")";
            var idsServicosForaDiretoria = dbConnection.Query<int>(query).ToList();
            return idsServicosForaDiretoria;
        }

        public List<ServicoMigracaoDTO> ObterServicosCompletos(List<int> idsServicos)
        {
            List<ServicoMigracaoDTO> servicosCompletos;
            var connectionStringEAcesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEAcesso))
            {
                dbConnection.Open();
                string query = @"
                        select cs.idservico, cs.nome as descricaoServico, cs.idcelula, cs.idcliente, cli.nomefantasia as nomecliente, cs.markup, cs.salesforceid as descricaoContrato, 
                        cs.DtCriacao as dtInicioContrato, cs.DtInativacao as dtFimContrato, 'REAL' as descricaoMoeda, cs.formafatur as descricaoformafaturamento,
                        cs.rentabilidade as rentabilidadePrevista, cs.vlReembKM as valorKmRodado, cs.idempresa as idColigada, cel.idtipocelula as idTipoCelula,
                        emp.nomeTxt as descricaoColigada, cs.IdFilial, fil.Filial as descricaoFilial, cs.Cod_Produto as codProdutoRm, cs.reembolso as isDespesasReembolsaveis,
                        cs.horasextraspgmensal as isHeReembolsaveis, cs.fatrecorrente as isFaturaRecorrente, cs.cReoneracao as isReeoneracao,
                        case cel.idtipocelula when 1 then 1 else 0 end as principal,
                        case cs.reembTipo when 1 then 'Nota de serviço' else 'Nota de débito' end as descricaoTipoReembolso 
                        from stfcorp.tblclientes cli inner join stfcorp.tblclientesservicos cs on cs.idcliente = cli.idcliente 
                            left join stfcorp.tblEmpresasGrupo emp on emp.idempresa = cs.idEmpresa
                            left join stfcorp.tblFiliais fil on fil.idfilial = cs.idfilial and fil.idempresa = cs.idempresa
                            left join stfcorp.tblCelulas cel on cel.IdCelula = cs.idcelula
                        where cs.idcliente = cli.idcliente and idservico in (" + string.Join(",", idsServicos) + ")";

                servicosCompletos = dbConnection.Query<ServicoMigracaoDTO>(query).ToList();
                dbConnection.Close();
            }
            return servicosCompletos;
        }

        public List<int> BuscarIdServicosPorNomeServico(string nome)
        {
            List<int> idServicos;
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                var query = $"SELECT s.idServico as id FROM stfcorp.tblClientesServicos s WHERE s.Nome LIKE '%{nome}%';";
                idServicos = dbConnection.Query<int>(query).ToList();
            }
            return idServicos;
        }

        public List<ComboDefaultDto> ObterServicosPorIdsEacesso(string servicos)
        {
            List<ComboDefaultDto> servicosDto;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select distinct s.IdServico as id, s.Nome as descricao from stfcorp.tblClientesServicos s, stfcorp.tblClientes c where s.IdServico in(" + servicos + ")  ORDER BY descricao;";
                servicosDto = dbConnection.Query<ComboDefaultDto>(query).ToList();
                dbConnection.Close();
            }
            return servicosDto;
        }

        public ComboClienteServicoDto ObterServicoPorId(int idServico)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select distinct s.IdServico as id, s.Nome as descricao, s.SiglaTipoServico as Categoria" +
                            " from stfcorp.tblClientesServicos s, stfcorp.tblClientes c " +
                            "where s.IdServico = " + idServico + ";";
                var servico = dbConnection.QueryFirstOrDefault<ComboClienteServicoDto>(query);
                dbConnection.Close();
                return servico;
            }
        }
    }
}
