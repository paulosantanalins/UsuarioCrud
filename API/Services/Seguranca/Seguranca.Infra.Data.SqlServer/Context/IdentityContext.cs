using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Seguranca.Domain.UsuarioRoot;

namespace Seguranca.Infra.Data.SqlServer.Context
{
    public class IdentityContext : IdentityDbContext<Usuario>
    {

        public IdentityContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}