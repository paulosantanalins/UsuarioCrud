using Cadastro.Domain.EnderecoRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class AbreviaturaLogradouroRepository : BaseRepository<AbreviaturaLogradouro>, IAbreviaturaLogradouroRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public AbreviaturaLogradouroRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }
    }
}
