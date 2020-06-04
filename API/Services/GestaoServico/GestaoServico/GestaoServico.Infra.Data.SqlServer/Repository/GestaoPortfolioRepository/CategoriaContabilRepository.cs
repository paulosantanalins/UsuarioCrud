using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Context;
using GestaoServico.Infra.Data.SqlServer.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GestaoServico.Infra.Data.SqlServer.Repository.GestaoPortfolioRepository
{
    public class CategoriaContabilRepository : BaseRepository<CategoriaContabil>, ICategoriaContabilRepository
    {
        public CategoriaContabilRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables) : base(gestaoServicoContext, variables)
        {

        }

        public FiltroGenericoDto<CategoriaContabil> Filtrar(FiltroGenericoDto<CategoriaContabil> filtro)
        {
            var query = DbSet.AsNoTracking().AsQueryable();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                var stringAtivo = "ATIVO";
                var stringInativo = "INATIVO";

                query = query.Where(x =>
                    x.Id.ToString().ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.DescCategoria.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.SgCategoriaContabil.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    (x.FlStatus == true && stringAtivo.Equals(filtro.ValorParaFiltrar.ToUpper())) ||
                    (x.FlStatus == false && stringInativo.Equals(filtro.ValorParaFiltrar.ToUpper())));
            }

            var dados = query.Select(p => new CategoriaContabil
            {
                Id = p.Id,
                DescCategoria = p.DescCategoria,
                SgCategoriaContabil = p.SgCategoriaContabil,
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                FlStatus = p.FlStatus,
            });
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

        public bool Validar(CategoriaContabil categoria)
        {
            var result = DbSet.Where(x => x.SgCategoriaContabil.ToUpper() == categoria.SgCategoriaContabil.ToUpper() && x.FlStatus && categoria.Id != x.Id).Any();
            return result;
        }

        public List<CategoriaContabil> ObterAtivos()
        {
            var result = DbSet.Where(x => x.FlStatus).ToList();
            return result;
        }

        public bool ValidarInexistencia(int id)
        {
            var result = DbSet.AsQueryable().Include(x => x.ClassificacoesContabil);

            var dados = DbSet.Any(x => x.Id == id && !x.ClassificacoesContabil.Any(y => y.FlStatus));
            return dados;
        }
    }
}
