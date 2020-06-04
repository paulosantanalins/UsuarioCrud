using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Connections;

namespace GestaoServico.Api.Jobs.Helper
{
    public class ConnectionHelper
    {
        protected readonly IOptions<ConnectionStrings> _connectionStrings;
        public ConnectionHelper(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public string GetConnectionEacesso()
        {
            return _connectionStrings.Value.EacessoConnection;
        }

    }
}
