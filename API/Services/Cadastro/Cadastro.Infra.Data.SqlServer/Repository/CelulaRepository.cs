using System.Collections.Generic;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Microsoft.EntityFrameworkCore;
using Utils;
using System.Linq;
using Cadastro.Infra.Data.SqlServer.Repository;
using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Logger.Repository.Interfaces;
using Cadastro.Infra.Data.SqlServer.Context;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class CelulaRepository : BaseRepository<Celula>, ICelulaRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public CelulaRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }

        public List<Celula> BuscarCelulasGerente(List<int> celulasId)
        {
            var result = DbSet.AsNoTracking().Where(x => celulasId.Any(y => y == x.Id)).ToList();
            return result;
        }

        public List<Celula> BuscarCelulasDiretor(List<int> celulasId)
        {
            var result = DbSet.Include(x => x.CelulaSuperior).AsNoTracking().Where(x => celulasId.Any(y => y == x.Id)).Select(x => x.CelulaSuperior).ToList();
            return result;
        }

        public List<Celula> BuscarTodosAtivasComInclude()
        {
            var result = DbSet.AsQueryable()
                .Include(x => x.CelulaSuperior)
                .Include(x => x.Grupo)
                .Where(x => x.Status != SharedEnuns.StatusCelula.Inativada.GetHashCode())
                .AsNoTracking();

            return result.ToList();
        }

        public Celula BuscarPorIdAtivasComInclude(int idCelula)
        {
            var result = DbSet.AsQueryable()
                .Include(x => x.CelulaSuperior)
                .Include(x => x.Grupo)
                .Include(x => x.Pessoa)
                .Where(x => idCelula == x.Id)
                .FirstOrDefault();

            return result;
        }
    }
}
