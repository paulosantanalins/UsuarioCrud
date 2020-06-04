using Cadastro.Domain.LinkRoot.Entity;
using Cadastro.Domain.LinkRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class LinkRepository : BaseRepository<Link>, ILinkRepository
    {
        public LinkRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }
    }
}
