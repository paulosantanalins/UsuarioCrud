using Cadastro.Domain.PessoaRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PessoaRoot.Repository
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        int? ObterIdPessoa(int? idEacesso);
        IEnumerable<string> ObterTodosCPFs();
        Pessoa BuscarPessoaComIncludeEnderecoTelefone(string cpf);
    }
}
