using UsuarioApi.Infra.Data.SqlServer.Context;
using UsuarioApi.Domain.DominioRoot.Repository;

namespace UsuarioApi.Infra.Data.SqlServer.Repository
{
    public class UsuarioRepository : BaseRepository<Domain.DominioRoot.Entity.Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(
            AppDbContext context) : base(context)
        {

        }


    }
}
