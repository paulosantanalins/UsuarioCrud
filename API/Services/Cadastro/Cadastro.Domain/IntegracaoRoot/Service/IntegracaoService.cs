using Cadastro.Domain.IntegracaoRoot.Service.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Utils;

namespace Cadastro.Domain.IntegracaoRoot.Service
{
    public class IntegracaoService : IIntegracaoService
    {
        public bool Execute(string query)
        {
            if (!Variables.EnvironmentName.Equals("Production", StringComparison.InvariantCultureIgnoreCase))
            {
                var connectionString = Variables.DefaultConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Execute(query);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public dynamic Select(string query)
        {
            if (!Variables.EnvironmentName.Equals("Production", StringComparison.InvariantCultureIgnoreCase))
            {
                var connectionString = Variables.DefaultConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionString))
                {
                    var result = dbConnection.Query(query);
                    return result;
                }
            }
            else
            {
                return "";
            }
        }

        public dynamic SelectEacesso(string query)
        {
            if (!Variables.EnvironmentName.Equals("Production", StringComparison.InvariantCultureIgnoreCase))
            {
                var connectionString = Variables.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionString))
                {
                    var result = dbConnection.Query(query);
                    return result;
                }
            }
            else
            {
                return "";
            }
        }
    }
}
