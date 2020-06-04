using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class HorasMesRepository : BaseRepository<HorasMes>, IHorasMesRepository
    {
        public HorasMesRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
           : base(context, variables, auditoriaRepository)
        {

        }

        public List<HorasMes> BuscarPeriodos()
        {
            var result = DbSet
                .Include(x => x.PeriodosDiaPagamento)
                    .ThenInclude(x => x.DiaPagamento)
                .Where(x => x.Ativo)
                .OrderByDescending(x => x.Ano).ThenByDescending(x => x.Mes)
                .ToList();

            return result;
        }

        public HorasMes BuscarPeriodoVigente()
        {
             var result = DbSet
                .Include(x => x.PeriodosDiaPagamento)
                    .ThenInclude(x => x.DiaPagamento)
                .Where(x => x.Ativo)
                .OrderByDescending(x => x.Ano)
                    .ThenByDescending(x => x.Mes)
                .FirstOrDefault();

            return result;
        }
    }
}
