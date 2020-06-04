using GestaoServico.Domain.GestaoPortifolioRoot.DTO;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPortfolioRepository
{
    public class EscopoServicoRepository : BaseRepository<EscopoServico>, IEscopoServicoRepository
    {
        public EscopoServicoRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables) : base(gestaoServicoContext, variables)
        {

        }

        public bool VerificarExistenciaEscopoServicoIgual(EscopoServico escopoServico)
        {
            var result = DbSet.Any(x => x.NmEscopoServico.ToUpper() == escopoServico.NmEscopoServico.ToUpper() && x.Id != escopoServico.Id && x.FlAtivo);

            return result;
        }

        public bool VerificarExclusaoValida(int idEscopo)
        {
            var result = DbSet.Include(x => x.ServicoContratados)
                            .Any(x => x.Id == idEscopo && !x.ServicoContratados.Any());

            return result;
        }

        public FiltroGenericoDto<GridEscopoDTO> Filtrar(FiltroGenericoDto<GridEscopoDTO> filtro)
        {
            var query = DbSet.Include(x => x.PortfolioServico)
                             .AsNoTracking();
            var stringAtivo = "ATIVO";
            var stringInativo = "INATIVO";
            if (filtro.ValorParaFiltrar != null)
            {
                query = query
                    .Where(x => x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                    || x.NmEscopoServico.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                    || x.PortfolioServico.NmServico.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                    || (
                        x.FlAtivo == true && stringAtivo.Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                        (x.FlAtivo == false && stringInativo.Contains(filtro.ValorParaFiltrar.ToUpper()))
                        )
                    || (
                        x.DataAlteracao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                        x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                       )
                    );
            }

            filtro.Total = query.Count();
            var dados = query.Select(p => new GridEscopoDTO
            {
                NmEscopoServico = p.NmEscopoServico,
                NmPortifolioServico = p.PortfolioServico.NmServico,
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                Id = p.Id,
                FlAtivo = p.FlAtivo
            });

            dados.OrderBy(x => x.FlAtivo == true).ThenBy(x => x.Id);
            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                dados = dados.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                dados = dados.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x));
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        public IEnumerable<MultiselectDto> ObterAtivos()
        {
            var result = DbSet.Where(x => x.FlAtivo).Select(x => new MultiselectDto
            {
                Id = x.Id,
                Nome = x.NmEscopoServico,
                IdSecundario = x.IdPortfolioServico
            });

            return result;
        }

        public IEnumerable<MultiselectDto> ObterAtivosPorPortfolio(int idPortfolio)
        {
            var result = DbSet.Where(x => x.FlAtivo && x.IdPortfolioServico == idPortfolio).Select(x => new MultiselectDto
            {
                Id = x.Id,
                Nome = x.NmEscopoServico,
                IdSecundario = x.IdPortfolioServico
            });

            return result;
        }

        private IQueryable<EscopoServico> FiltrarDadosAlteracao(FiltroGenericoDto<GridEscopoDTO> filtro, IQueryable<EscopoServico> query)
        {
            return query.Where(x => x.DataAlteracao.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                                x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
        }

        private static IQueryable<EscopoServico> FiltrarNomePortfolio(FiltroGenericoDto<GridEscopoDTO> filtro, IQueryable<EscopoServico> query)
        {
            return query.Where(x => x.PortfolioServico.NmServico.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
        }

        private static IQueryable<EscopoServico> FiltrarNomeEscopo(FiltroGenericoDto<GridEscopoDTO> filtro, IQueryable<EscopoServico> query)
        {
            return query.Where(x => x.NmEscopoServico.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
        }

        private static IQueryable<EscopoServico> FiltrarId(FiltroGenericoDto<GridEscopoDTO> filtro, IQueryable<EscopoServico> query)
        {
            return query.Where(x => x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
        }

        private static IQueryable<EscopoServico> FiltrarStatus(FiltroGenericoDto<GridEscopoDTO> filtro, IQueryable<EscopoServico> query)
        {
            var stringAtivo = "ATIVO";
            var stringInativo = "INATIVO";

            return query.Where(x => x.FlAtivo == true && stringAtivo.Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                               (x.FlAtivo == false && stringInativo.Contains(filtro.ValorParaFiltrar.ToUpper())));
        }
    }
}
