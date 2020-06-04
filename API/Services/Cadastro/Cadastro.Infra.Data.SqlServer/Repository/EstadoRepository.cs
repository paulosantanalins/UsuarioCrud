using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.EmpresaGrupoRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class EstadoRepository : BaseRepository<Estado> , IEstadoRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public EstadoRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }

        public IEnumerable<Estado> BuscarEstadosBrasileiros()
        {
            var result = DbSet
                        .Include(x => x.Pais)
                        .Where(x => x.Pais.SgPais == "BRA")                        
                        .OrderBy(x => x.NmEstado);

            return result;
        }
    }
}
