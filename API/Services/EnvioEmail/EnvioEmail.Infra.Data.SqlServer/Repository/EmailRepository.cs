using System.Collections.Generic;
using System.Linq;
using EnvioEmail.Domain.EmailRoot.Entity;
using EnvioEmail.Domain.EmailRoot.Repository;
using EnvioEmail.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace EnvioEmail.Infra.Data.SqlServer.Repository
{
    public class EmailRepository : BaseRepository<Email>, IEmailRepository
    {
        public EmailRepository(ServiceBContext context) : base(context)
        {
            
        }

        public IEnumerable<Email> BuscarEmailsPendentes()
        {
            var result = DbSet.Where(x => x.Status == "E" || x.Status == "A").AsNoTracking();
            return result;
        }
    }
}
