using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Utils.Connections
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
