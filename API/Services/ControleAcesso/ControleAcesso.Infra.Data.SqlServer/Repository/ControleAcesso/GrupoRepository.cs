using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Utils;
using Utils.Base;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.SharedRoot;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class GrupoRepository : BaseRepository<Grupo>, IGrupoRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public GrupoRepository(ControleAcessoContext context, IVariablesToken variables)
            : base(context, variables)
        {
            _variables = variables;
            //_auditoriaRepository = auditoriaRepository;
        }

        public FiltroGenericoDtoBase<GrupoDto> FiltrarGrupos(FiltroGenericoDtoBase<GrupoDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

            query = filtro.Id == -1 ? query :
                filtro.Id == 1 ? query.Where(x => x.Ativo) :
                query.Where(x => !x.Ativo);

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar) && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x => x.Id.ToString().ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper())
                                  || x.DescGrupo.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                  || (filtro.ValorParaFiltrar.ToUpper().Equals(x.Ativo ? "ATIVO" : "INATIVO")));
            }
            
            var dados = query.Select(x => new GrupoDto
            {
                Id = x.Id,
                DataAlteracao = x.DataAlteracao,
                Ativo = x.Ativo,
                Descricao = x.DescGrupo,
                Usuario = x.Usuario
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
                dados = dados.OrderBy(x => x.Descricao);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            
            return filtro;
        }

        public bool ValidarExisteGrupo(string descricao)
        {
            var result = DbSet.AsQueryable().Where(x => x.DescGrupo.Equals(descricao) && x.Ativo);
            return result.Count() > 0;
        }

        public bool ExisteGrupoComCelulasInativas(int idGrupo)
        {
            var result = DbSet.AsQueryable()
                .Include(x => x.Celulas)
                .Where(x => idGrupo == x.Id && x.Ativo)
                .FirstOrDefault();
            if (result != null)
            {
                if (result.Celulas.Count > 0 && result.Celulas.Any(c => c.Status != SharedEnuns.StatusCelula.Inativada.GetHashCode()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
