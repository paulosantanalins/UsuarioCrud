using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.Jobs
{
    public class SqlDbConection
    {
        public static string ConnectionString { get; set; }

        public static IDbConnection Connection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }
    }
}
