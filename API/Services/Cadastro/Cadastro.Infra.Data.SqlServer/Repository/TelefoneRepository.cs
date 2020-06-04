using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using Cadastro.Domain.TelefoneRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class TelefoneRepository : BaseRepository<Telefone>, ITelefoneRepository
    {
        public TelefoneRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }
    }
}
