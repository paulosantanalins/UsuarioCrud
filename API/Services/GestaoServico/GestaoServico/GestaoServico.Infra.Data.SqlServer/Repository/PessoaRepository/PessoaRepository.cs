using GestaoServico.Domain.PessoaRoot.Entity;
using GestaoServico.Domain.PessoaRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.PessoaRepository
{
    public class PessoaRepository : BaseRepository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables)
            : base(gestaoServicoContext, variables)
        {

        }

        public List<int?> BuscarTodosIdsEacesso()
        {
            var ids = DbSet.AsNoTracking().Select(x => x.IdEacesso).ToList();
            return ids;
        }

        public Pessoa Buscar(int? idEacesso)
        {
            return DbSet.FirstOrDefault(x => x.IdEacesso == idEacesso);
        }
    }
}
