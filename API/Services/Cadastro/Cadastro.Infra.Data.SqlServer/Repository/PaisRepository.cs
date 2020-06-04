using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using System.Collections.Generic;
using Cadastro.Domain.SharedRoot;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class PaisRepository : BaseRepository<Pais>, IPaisRepository
    {
        protected readonly IVariablesToken _variables;
        private readonly IAuditoriaRepository _auditoriaRepository;

        public PaisRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }


        public void SalvarPaises(List<Pais> paises)
        {
            DbSet.AddRange(paises);
            _variables.UserName = "salesForce";
            _context.SaveChanges();
        }
    }
}
