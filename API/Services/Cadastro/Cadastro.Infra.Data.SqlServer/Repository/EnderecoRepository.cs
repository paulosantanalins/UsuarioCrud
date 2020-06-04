using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class EnderecoRepository : BaseRepository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }
    }
}
