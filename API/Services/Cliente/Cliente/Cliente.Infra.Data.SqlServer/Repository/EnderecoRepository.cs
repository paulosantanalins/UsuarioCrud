using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.ClienteRoot.Repository;
using Cliente.Infra.Data.SqlServer.Context;
using Cliente.Infra.Data.SqlServer.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cliente.Domain.SharedRoot;
using Utils;

namespace Cliente.Infra.Data.SqlServer.Repository
{
    public class EnderecoRepository : BaseRepository<Endereco>, IEnderecoRepository
    {
        private readonly ClienteContext _clienteContext;

        public EnderecoRepository(ClienteContext clienteContext, IVariablesToken variables): base(clienteContext, variables)
        {
        }
    }
}
