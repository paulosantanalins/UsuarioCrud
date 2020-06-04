using GestaoServico.Domain.Dto;
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
    public class ClassificacaoContabilRepository : BaseRepository<ClassificacaoContabil>, IClassificacaoContabilRepository
    {
        
        public ClassificacaoContabilRepository(GestaoServicoContext gestaoServicoContext, IVariablesToken variables) : base(gestaoServicoContext, variables)
        {
            
        }

        public bool Validar(ClassificacaoContabil model)
        {
            var result = DbSet.Where(x => x.SgClassificacaoContabil.ToUpper() == model.SgClassificacaoContabil.ToUpper() && x.FlStatus && model.Id != x.Id).Any();
            return result;
        }

        public bool ValidarDescricao(ClassificacaoContabil model)
        {
            var result = DbSet.Where(x => x.DescClassificacaoContabil.ToUpper() == model.DescClassificacaoContabil.ToUpper() && x.FlStatus && model.Id != x.Id).Any();
            return result;
        }

        public FiltroGenericoDto<ClassificacaoContabilDto> Filtrar(FiltroGenericoDto<ClassificacaoContabilDto> filtro)
        {
            var query = DbSet.AsNoTracking()
               .Include(x => x.CategoriaContabil).AsQueryable();

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                var stringAtivo = "ATIVO";
                var stringInativo = "INATIVO";

                query = query.Where(x =>
                    x.Id.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.SgClassificacaoContabil.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.DescClassificacaoContabil.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.CategoriaContabil.SgCategoriaContabil.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    x.CategoriaContabil.DescCategoria.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||                   
                    x.Usuario.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                    (x.FlStatus == true && stringAtivo.Equals(filtro.ValorParaFiltrar.ToUpper())) ||
                    (x.FlStatus == false && stringInativo.Equals(filtro.ValorParaFiltrar.ToUpper())));
            }

            var dados = query.Select(p => new ClassificacaoContabilDto
            {
                Id = p.Id,
                SgClassificacaoContabil = p.SgClassificacaoContabil,
                DescClassificacaoContabil = p.DescClassificacaoContabil,
                IdCategoriaContabil = p.CategoriaContabil.Id,
                Usuario = p.Usuario,
                DataAlteracao = p.DataAlteracao,
                FlStatus = p.FlStatus,
                DescCategoria = p.CategoriaContabil.DescCategoria,           
                SgCategoria = p.CategoriaContabil.SgCategoriaContabil
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

        public ICollection<ClassificacaoContabil> BuscarClassificacoes(int id)
        {
            var result = DbSet.Where(x => x.IdCategoriaContabil == id && x.FlStatus).Select(x => new ClassificacaoContabil {Id = x.Id, DescClassificacaoContabil = x.DescClassificacaoContabil }).ToList();
            return result;
        }

        public bool ValidarInexistencia(int id)
        {
            var result = DbSet.AsQueryable().Include(x => x.PortifolioServicos);
            var dados = DbSet.Any(x => x.Id == id && !x.PortifolioServicos.Any(y => y.FlStatus));
            return dados;
        }
    }
}
