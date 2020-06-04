using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class DocumentoPrestadorRepository : BaseRepository<DocumentoPrestador>, IDocumentoPrestadorRepository
    {
        public DocumentoPrestadorRepository(CadastroContexto context,
            IVariablesToken variables, IAuditoriaRepository
            auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

    
    }
}
