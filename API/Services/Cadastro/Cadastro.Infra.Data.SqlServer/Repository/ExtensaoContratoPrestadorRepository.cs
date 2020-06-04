using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class ExtensaoContratoPrestadorRepository : BaseRepository<ExtensaoContratoPrestador>, IExtensaoContratoPrestadorRepository
    {
        public ExtensaoContratoPrestadorRepository(CadastroContexto context,
            IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }


    }
}
