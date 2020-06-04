using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Cadastro.Domain.FilialRoot.Dto;
using Cadastro.Domain.FilialRoot.Service.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using Utils.Connections;

namespace Cadastro.Domain.FilialRoot.Service
{
    public class FilialService : IFilialService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public FilialService(
            IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public IEnumerable<FilialRmDto> BuscarNoRm(int idColigada)
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                string sQueryInsercao = "SELECT CODFILIAL as Id, NOMEFANTASIA as Descricao FROM GFILIAL " +
                        "WHERE CODCOLIGADA = " + idColigada + " AND NOMEFANTASIA != 'INATIVO' AND NOMEFANTASIA != 'INATIVA'" +
                        " AND NOMEFANTASIA != 'SUSPENSO' AND NOMEFANTASIA != 'SUSPENSA' ORDER BY NOMEFANTASIA;";

                var result = dbConnection.Query<FilialRmDto>(sQueryInsercao);
                
                return result;
            }
        }

        public FilialRmDto BuscarFilialNoRm(int idFilial, int idColigada)
        {
            var connectionString = _connectionStrings.Value.RMConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();

                string sQueryInsercao = $@"SELECT CODFILIAL as Id, NOMEFANTASIA as Descricao FROM GFILIAL
                                        WHERE CODFILIAL = {idFilial} AND CODCOLIGADA = {idColigada} AND NOMEFANTASIA != 'INATIVO' AND NOMEFANTASIA != 'INATIVA'
                                        AND NOMEFANTASIA != 'SUSPENSO' AND NOMEFANTASIA != 'SUSPENSA' ORDER BY NOMEFANTASIA;";

                var result = dbConnection.QueryFirstOrDefault<FilialRmDto>(sQueryInsercao);
                dbConnection.Close();


                return result;
            }
        }
    }
}
