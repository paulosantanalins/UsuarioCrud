using ControleAcesso.Domain.DominioRoot.Entity;
using ControleAcesso.Domain.DominioRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using System.Linq;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class DominioRepository : BaseRepository<Dominio>, IDominioRepository
    {
        public DominioRepository(ControleAcessoContext context,
            IVariablesToken variables) : base(context, variables)
        {
            
        }

        public int? ObterIdPeloCodValor(int idValor, string tipoDominio)
        {
            if (idValor != 0)
            {
                var result = DbSet.FirstOrDefault(x => x.IdValor == idValor && x.ValorTipoDominio.Equals(tipoDominio));
                return result.Id;
            }
            else
            {
                return null;
            }
        }

        public int ObterIdDominio(string tipoDominio, string valorDominio)
        {
            var result = DbSet.FirstOrDefault(x => x.ValorTipoDominio.Equals(tipoDominio) && x.DescricaoValor.Equals(valorDominio)); ;
            return result.Id;
        }
    }
}
