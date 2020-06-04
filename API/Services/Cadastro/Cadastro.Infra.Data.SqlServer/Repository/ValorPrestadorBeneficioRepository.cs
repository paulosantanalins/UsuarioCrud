using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class ValorPrestadorBeneficioRepository : BaseRepository<ValorPrestadorBeneficio> , IValorPrestadorBeneficioRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public ValorPrestadorBeneficioRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }

        public List<ValorPrestadorBeneficio> BuscarValoresPrestadorBeneficio(int idValorPrestador)
        {
            var result = DbSet
                .Include(x => x.Beneficio)
                .Where(x => x.IdValorPrestador == idValorPrestador)                
                .ToList();

            return result;
        }
    }
}
