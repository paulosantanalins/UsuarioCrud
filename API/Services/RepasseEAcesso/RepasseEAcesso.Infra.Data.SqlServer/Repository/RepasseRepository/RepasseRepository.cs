using Microsoft.EntityFrameworkCore;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace RepasseEAcesso.Infra.Data.SqlServer.Repository.RepasseRepository
{
    public class RepasseRepository : BaseLegadoRepository<Repasse>, IRepasseRepository
    {
        public RepasseRepository(RepasseLegadoContext context, IVariablesToken variables) : base (context, variables)
        {
            

        }

        public List<Repasse> BuscarTodosMigracaoRepasse()
        {
            var result = DbSet
                        .AsQueryable()
                        .Where(x => x.Status == "NA").ToList();

            return result;
        }

        public List<Repasse> BuscarTodosFilhosLegado(int idRepasseEacesso)
        {
            var query = DbSet
                        .Where(x => x.IdRepasseMae == idRepasseEacesso)
                        .AsNoTracking().ToList();

            return query.ToList();
        }
    }
}
