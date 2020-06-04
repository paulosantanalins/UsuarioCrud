using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class LogHorasMesPrestadorRepository : BaseRepository<LogHorasMesPrestador>, ILogHorasMesPrestadorRepository
    {
        public LogHorasMesPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }

        public List<LogHorasMesPrestador> BuscarLogsPorPrestador(HorasMesPrestador prestadorHoras)
        {
            var result = DbSet
                            .Include(x => x.HorasMesPrestador).AsNoTracking()
                            .Where(x => x.HorasMesPrestador.IdHorasMes == prestadorHoras.IdHorasMes && x.HorasMesPrestador.IdPrestador == prestadorHoras.IdPrestador)
                            .ToList();
            return result;
        }
    }
}
