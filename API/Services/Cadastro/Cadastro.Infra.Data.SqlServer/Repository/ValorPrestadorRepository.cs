using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Utils;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class ValorPrestadorRepository : BaseRepository<ValorPrestador>, IValorPrestadorRepository
    {
        public ValorPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }

        public List<ValorPrestador> BuscarComIncludePrestador()
        {
            var result = DbSet.AsNoTracking()
                    .Include(x => x.Prestador)
                    .Where(x => x.Prestador.CodEacessoLegado.HasValue)
                    .ToList();
            return result;
        }

        public List<ValorPrestador> BuscarTodosPorIdPrestador(int idPrestador)
        {
            var result = DbSet.AsNoTracking()
                    .Where(x => idPrestador == x.IdPrestador)
                    .ToList();
            return result;
        }
    }
}
