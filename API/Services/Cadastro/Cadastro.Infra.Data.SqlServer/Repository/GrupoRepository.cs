using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;
using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.SharedRoot;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class GrupoRepository : BaseRepository<Grupo>, IGrupoRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public GrupoRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;

        }
    }
}
