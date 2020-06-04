using Cadastro.Domain.EmpresaGrupoRoot.Dto;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils.Connections;

namespace Cadastro.Domain.EmpresaGrupoRoot.Service
{
    public class EmpresaGrupoService : IEmpresaGrupoService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public EmpresaGrupoService(
            IOptions<ConnectionStrings> connectionStrings
            )
        {
            _connectionStrings = connectionStrings;
        }

        public IEnumerable<EmpresaGrupoRmDto> BuscarNoRM()
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();

                string sQueryInsercao = "SELECT CODCOLIGADA as Id, NOMEFANTASIA as Descricao FROM GCOLIGADA WHERE CODCOLIGADA IN ( " +
                        "SELECT CODCOLIGADA FROM GCOLIGADA EXCEPT SELECT " +
                        "CAST(CODINTERNO AS BIGINT) CODCOLIGADA FROM GCONSIST WHERE APLICACAO = 'C' AND CODTABELA = 'GCOLSEMMOV' )";

                var result = dbConnection.Query<EmpresaGrupoRmDto>(sQueryInsercao);
                dbConnection.Close();
                return result.OrderBy(x => x.Descricao);
            }
        }

        public EmpresaGrupoRmDto BuscarNoRMPorId(int id, bool pagarPelaMatriz, int idFilial)
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();

                string sQueryInsercao = "SELECT CODCOLIGADA as Id, NOMEFANTASIA as Descricao, NOME as Nome, cgc as cnpj, inscricaoestadual FROM GCOLIGADA where codcoligada = " + id;
                var result = dbConnection.Query<EmpresaGrupoRmDto>(sQueryInsercao).FirstOrDefault();

                if (!pagarPelaMatriz)
                {
                    sQueryInsercao = "SELECT cgc as cnpj, inscricaoestadual FROM GFILIAL where codcoligada = " + id + "and codfilial = " + idFilial + "";

                    var resultFilial = dbConnection.Query<EmpresaGrupoRmDto>(sQueryInsercao).FirstOrDefault();
                    if (resultFilial != null)
                    {
                        result.Cnpj = resultFilial.Cnpj;
                        result.InscricaoEstadual = resultFilial.InscricaoEstadual;
                    }
                }

                dbConnection.Close();
                return result;
            }
        }

        public EmpresaGrupoRmDto BuscarNoRMPorId(int id)
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();

                string sQueryInsercao = "SELECT CODCOLIGADA as Id, NOMEFANTASIA as Descricao, NOME as Nome, cgc as cnpj, inscricaoestadual FROM GCOLIGADA where codcoligada = " + id;

                var result = dbConnection.Query<EmpresaGrupoRmDto>(sQueryInsercao).FirstOrDefault();
                dbConnection.Close();
                return result;
            }
        }

        public DadosBancariosDaEmpresaGrupoDto BuscarDadosBancariosDaEmpresaDoGrupoPorIdEmpresaGrupoEBanco(int idEmpresaGrupo, string banco)
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();

                string sQueryInsercao = $@"SELECT FCXA.CODCOLIGADA AS ID, 
                                          COL.NOMEFANTASIA AS EMPRESA,
	                                      COL.NOME AS DESCRICAO,
                                          FCXA.CODCXA,
                                          GBAN.NOME AS BANCO,
	                                      FCONTA.NUMBANCO as DIGITODOBANCO,
	                                      FCONTA.NUMAGENCIA as AGENCIA,
	                                      FCONTA.NROCONTA AS CONTA,
	                                      FCONTA.DIGCONTA as DIGITODACONTA
                                          FROM GBANCO GBAN, FCXA
                                          INNER JOIN FCONTA
                                          ON(FCXA.CODCOLCONTA = FCONTA.CODCOLIGADA AND FCXA.NROCONTA = FCONTA.NROCONTA
                                          AND FCXA.NUMBANCO = FCONTA.NUMBANCO AND FCXA.NUMAGENCIA = FCONTA.NUMAGENCIA)
                                          INNER JOIN GCOLIGADA COL
                                          ON FCONTA.CODCOLIGADA = COL.CODCOLIGADA
                                          WHERE FCXA.NUMBANCO = GBAN.NUMBANCO
                                          AND CODCXA LIKE '1-%'
                                          AND FCXA.CODCOLIGADA = {idEmpresaGrupo}
                                          AND GBAN.NUMBANCO = '{(banco.ToUpper().Trim() == "SANTANDER" ? "033" : "341")}'
                                          AND FCXA.ATIVA = 1
                                          AND FCONTA.TIPOCONTA = 1;";

                var result = dbConnection.Query<DadosBancariosDaEmpresaGrupoDto>(sQueryInsercao).FirstOrDefault();
                dbConnection.Close();
                return result;
            }
        }

        public IEnumerable<EmpresaGrupoRmDto> BuscarTodasNoRM()
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                string sQueryInsercao = "SELECT CODCOLIGADA as Id, NOMEFANTASIA as Descricao, NOME as Nome FROM GCOLIGADA";
                var result = dbConnection.Query<EmpresaGrupoRmDto>(sQueryInsercao);
                dbConnection.Close();
                return result.OrderBy(x => x.Descricao);
            }
        }
    }
}
