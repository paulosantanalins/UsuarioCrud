using GestaoServico.Domain.Interfaces;
using GestaoServico.Domain.PessoaRoot.Entity;
using System.Collections.Generic;

namespace GestaoServico.Domain.PessoaRoot.Repository
{
    public interface IPessoaRepository : IBaseRepository<Pessoa>
    {
        Pessoa Buscar(int? idEacesso);
        List<int?> BuscarTodosIdsEacesso();
    }
}
