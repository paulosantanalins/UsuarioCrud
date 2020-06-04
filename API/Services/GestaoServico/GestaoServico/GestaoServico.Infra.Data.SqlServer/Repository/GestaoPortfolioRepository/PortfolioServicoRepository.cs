using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using GestaoServico.Infra.Data.SqlServer.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPortfolioRepository
{
    public class PortfolioServicoRepository : BaseRepository<PortfolioServico>, IPortfolioServicoRepository
    {
        public PortfolioServicoRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables) : base(gestaoServicoContext, variables)
        {

        }

        public FiltroGenericoDto<PortfolioServicoDto> Filtrar(FiltroGenericoDto<PortfolioServicoDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var query = DbSet.AsNoTracking();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                var stringAtivo = "ATIVO";
                var stringInativo = "INATIVO";

                query = query.Where(x =>
                    x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())||
                    x.NmServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.DescServico.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.DataAlteracao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    (x.FlStatus == true && stringAtivo.Equals(filtro.ValorParaFiltrar.ToUpper())) ||
                    (x.FlStatus == false && stringInativo.Equals(filtro.ValorParaFiltrar.ToUpper())));
            }

            var dados = query.Select(p => new PortfolioServicoDto
                {
                    Id = p.Id,
                    NmServico = p.NmServico,
                    Usuario = p.Usuario,
                    DataAlteracao = p.DataAlteracao,
                    DescServico = p.DescServico,
                    FlStatus = p.FlStatus,
                    IdDelivery = p.IdDelivery,                
                
                }
            );
            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else
            {
                dados = dados.OrderByDescending(x => x.FlStatus).ThenBy(x => x.Id);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public bool Validar(PortfolioServico portfolioServico)
        {
            var result = DbSet.Where(x => ((x.NmServico.ToUpper() == portfolioServico.NmServico.ToUpper() || 
                                            x.DescServico.ToUpper() == portfolioServico.DescServico.ToUpper())
                                            && x.FlStatus && portfolioServico.Id != x.Id)).Any();
            return result;
        }


        public bool ValidarInativacao(int id)
        {
            //validar
            var result = DbSet.Include(x => x.EscopoServicos);
            var dados = result.Any(x => x.Id == id && !x.EscopoServicos.Any());
            return dados;
        }

    }
}
