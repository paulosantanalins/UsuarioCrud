using RepasseEAcesso.Domain.DominioRoot.Entity;
using RepasseEAcesso.Domain.DominioRoot.Repository;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using System;
using System.Linq;
using Utils;

namespace RepasseEAcesso.Infra.Data.SqlServer.Repository.DominioRepository
{
    public class DominioRepository : BaseRepository<Dominio>, IDominioRepository
    {
        public DominioRepository(RepasseEAcessoContext context, IVariablesToken variables) : base(context, variables)
        {

        }

        public Dominio Buscar(int idValor, string tipoDominio)
        {
            var dominio = DbSet.FirstOrDefault(x => x.IdValor.Equals(idValor) && x.ValorTipoDominio.Equals(tipoDominio.Trim(), StringComparison.InvariantCultureIgnoreCase));
            return dominio;
        }
    }
}
