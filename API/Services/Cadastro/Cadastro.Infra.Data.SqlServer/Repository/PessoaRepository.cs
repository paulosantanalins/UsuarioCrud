using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.PessoaRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class PessoaRepository : BaseRepository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }

        public int? ObterIdPessoa(int? idEacesso)
        {
            if (idEacesso.HasValue)
            {
                var result = DbSet.Where(x => x.CodEacessoLegado == idEacesso.Value).FirstOrDefault();
                if (result != null)
                {
                    return result.Id;
                }
            }
            return null;
        }

        public IEnumerable<string> ObterTodosCPFs() => DbSet.Select(x => x.Cpf).ToList();
        public Pessoa BuscarPessoaComIncludeEnderecoTelefone(string cpf)
        {
            return DbSet.AsNoTracking()                
                   .Include(x => x.Telefone)
                   .Include(x => x.Endereco)
                   .FirstOrDefault(x => x.Cpf == cpf);
        }

    }
}
