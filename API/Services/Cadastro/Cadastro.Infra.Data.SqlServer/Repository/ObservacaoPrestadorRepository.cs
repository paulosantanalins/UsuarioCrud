using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class ObservacaoPrestadorRepository : BaseRepository<ObservacaoPrestador>, IObservacaoPrestadorRepository
    {
        public ObservacaoPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }

    }
}
