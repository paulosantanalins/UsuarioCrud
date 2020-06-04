using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPortfolioRepository
{
    public class TipoServicoRepository : BaseRepository<TipoServico>, ITipoServicoRepository
    {
        public TipoServicoRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables) : base(gestaoServicoContext, variables)
        {
            
        }

        public FiltroGenericoDto<TipoServico> Filtrar(FiltroGenericoDto<TipoServico> filtro)
        {
            var query = DbSet.AsNoTracking();
            var stringAtivo = "ATIVO";
            var stringInativo = "INATIVO";

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => 
                    x.DescTipoServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.Id.ToString().Equals(filtro.ValorParaFiltrar) ||
                    (x.FlStatus == true && stringAtivo.Equals(filtro.ValorParaFiltrar.ToUpper())) ||
                    (x.FlStatus == false && stringInativo.Equals(filtro.ValorParaFiltrar.ToUpper())));
            }
            filtro.Total = query.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                query = query.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                query = query.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else
            {
                query = query.OrderByDescending(x => x.FlStatus).ThenBy(x => x.Id);
            }

            filtro.Valores = query.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public bool Validar(TipoServico model)
        {
            var result = DbSet.Where(x => x.DescTipoServico.ToUpper() == model.DescTipoServico.ToUpper() && x.FlStatus && model.Id != x.Id).Any();
            return result;
        }

        public bool ValidarInativação(int id)
        {
            var result = DbSet.AsQueryable().Include(x => x.PortifolioServicos);

            return result.Any(x => x.Id == id && !x.PortifolioServicos.Any(y => y.FlStatus));
        }
    }
}
