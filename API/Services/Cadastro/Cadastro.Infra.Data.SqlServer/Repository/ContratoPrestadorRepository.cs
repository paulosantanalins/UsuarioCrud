using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class ContratoPrestadorRepository : BaseRepository<ContratoPrestador>, IContratoPrestadorRepository
    {
        public ContratoPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

        public ContratoPrestador BuscarContratoPorPeriodo(int idPrestador, DateTime? dataInicio)
        {
            var contratoPrestador = DbSet.FirstOrDefault(x => dataInicio <= x.DataFim && x.IdPrestador == idPrestador);
            return contratoPrestador;
        }
       
        public IEnumerable<ContratoPrestador> BuscarComIncludes(int idPrestador)
        {
            var contratoPrestador = DbSet
                .Include(x => x.ExtensoesContratoPrestador)
                .Where(x => x.IdPrestador == idPrestador && !x.Inativo);

            return contratoPrestador;
        }

        public ContratoPrestador BuscarContratoComIncludes(int idContrato)
        {
            var contratoPrestador = DbSet.AsNoTracking().Include(x => x.ExtensoesContratoPrestador).FirstOrDefault(x => x.Id == idContrato);
            return contratoPrestador;
        }



        public ContratoPrestador BuscarContratoNoPeriodoVigente(int idPrestador)
        {
            var contratoPrestador = DbSet.FirstOrDefault(x => DateTime.Now <= x.DataFim && x.IdPrestador == idPrestador);
            return contratoPrestador;
        }
    }
}
