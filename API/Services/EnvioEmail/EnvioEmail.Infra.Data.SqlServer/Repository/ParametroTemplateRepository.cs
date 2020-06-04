using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Repository;
using EnvioEmail.Infra.Data.SqlServer.Context;
using System.Linq;
using Utils;

namespace EnvioEmail.Infra.Data.SqlServer.Repository
{
    public class ParametroTemplateRepository : BaseRepository<ParametroTemplate>, IParametroTemplateRepository
    {
        public ParametroTemplateRepository(ServiceBContext context) : base(context)
        {

        }

        public int? ObterIdPeloNome(string nomeParametro, int idTemplate)
        {
            var result = DbSet.FirstOrDefault(x => x.NomeParametro.Equals(nomeParametro) && x.IdTemplate == idTemplate);

            if (result != null)
            {
                return result.Id;
            }
            else
            {
                return null;
            }
        }
    }
}