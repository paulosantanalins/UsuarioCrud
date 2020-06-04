using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Cadastro.Domain.PessoaRoot.Dto;
using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using Utils.Connections;

namespace Cadastro.Domain.PessoaRoot.Service
{
    public class ViewProfissionaisService : IViewProfissionaisService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ViewProfissionaisService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public IEnumerable<ViewProfissionalGeral> ViewProfissionalGeralEacesso()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                dbConnection.Open();

                var query = $@"SELECT 
                                   [Celula]
                                  ,[Gerente]
                                  ,[CPFGerente]
                                  ,[Chapa]
                                  ,[Nome]
                                  ,[CPF]
                                  ,[Email]
                                  ,[Email_Int] as EmailInterno
                                  ,[Inativo]
                                  ,[Cargo]
                                  ,[TelCom]
                                  ,[TelRes]
                                  ,[Celular]
                                  ,[CelularCorporativo]
                                  ,[CIDADEPROFISSIONAL]
                                  ,[ESTADOPROFISSIONAL]
                                  ,[PAISPROFISSIONAL]
                                  ,[LocalTrabalho]
                                  ,[Contratacao]
                                  ,[DataNascimento]
                                  ,[1Nivel] as PrimeiroNivel
                                  ,[DataContratacao]
                                  ,[EmailGerente]
                                  ,[Coligada]
                                  ,[CidadeColigada]
                                  ,[EstadoColigada]
                                  ,[FILIAL]
                                  ,[ENDERECO]
                                  ,[NUMERO]
                                  ,[CEP]
                                  ,[BAIRRO]
                                  ,[CIDADE]
                                  ,[ESTADO]
                                  ,[PAIS]
                                  ,[SITUACAO]
                                  ,[IdProfissional]
                                  ,[Email_Alias] as EmailAlias
                                  ,[Area]
                                  ,[IdNivel]
                                  ,[Nivel]
                                  ,[ClienteCusto]
                                  ,[IdServicoCusto]
                                  ,[CodSindicato]
                                  ,[NomeSindicato]
                              FROM [stfcorp].[vwProfissionaisGeral]";

                var result = dbConnection.Query<ViewProfissionalGeral>(query);

                return result;
            }
        }

        public IEnumerable<ViewProfissionaisDesligados> ViewProfissionaisDesligados30Dias()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                dbConnection.Open();

                var query = $@"SELECT 
                                   [NOME]
                                  ,[CPF]
                                  ,[DTDESLIGAMENTO] as DataDesligamento
                              FROM [stfcorp].[VW_PROFDESLIGADOS30]";

                var result = dbConnection.Query<ViewProfissionaisDesligados>(query);

                return result;
            }
        }

        public IEnumerable<ViewAdimitidosFuturos> ViewAdimitidosFuturosNatCorp()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                dbConnection.Open();

                var query = $@"SELECT 
	                            Cpf
	                            ,Nome
	                            ,Email
	                            ,Requisitante
	                            ,NomeSocial
	                            ,Cod_Sindicato as CodSindicato
	                            ,Estado
	                            ,CodVaga
	                            ,CodColigada
	                            ,NomeColigada
	                            ,Celula
	                            ,GestorCelula
	                            ,CelulaSuperior
	                            ,GestorSuperior
                                FROM OPENQUERY([NATCORP_P],
                                'SELECT *
	                            from STEFANINI.VW_ADMITIDOS_FUTUROS')";

                var result = dbConnection.Query<ViewAdimitidosFuturos>(query);

                return result;
            }
        }

        public IEnumerable<ViewProfissionaisDesligados> ViewInativadosNatCorp()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                dbConnection.Open();

                var query = $@"SELECT 
	                            *
                                FROM OPENQUERY([NATCORP_P],
                                'SELECT *
	                            from STEFANINI.VW_INATIVOS')";

                var result = dbConnection.Query<ViewProfissionaisDesligados>(query);

                return result;
            }
        }

        public IEnumerable<ViewDemissaoFuturas> ViewDemissaoFuturas()
        {
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                dbConnection.Open();

                var query = $@"SELECT 
	                            Nome
	                            ,Chapa
	                            ,Cpf
	                            ,DataDeslig as DataDesligamento
	                            ,Gerente
	                            ,Coligada
                                FROM OPENQUERY([NATCORP_P],
                                'SELECT *
	                            from STEFANINI.VW_DEMISSAO_FUTUROS')";

                var result = dbConnection.Query<ViewDemissaoFuturas>(query);

                return result;
            }
        }
    }
}
