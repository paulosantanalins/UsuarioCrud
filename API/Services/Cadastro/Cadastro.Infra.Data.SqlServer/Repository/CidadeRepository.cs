using Cadastro.Domain.CidadeRoot.Dto;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.EnderecoRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;
using Utils.Base;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class CidadeRepository : BaseRepository<Cidade>, ICidadeRepository
    {
        protected readonly IVariablesToken _variables;
        protected readonly IAuditoriaRepository _auditoriaRepository;

        public CidadeRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {
            _variables = variables;
            _auditoriaRepository = auditoriaRepository;
        }

        public int? ObterIdPeloNome(string nome)
        {
            var cidade = DbSet.FirstOrDefault(x => x.NmCidade.Trim().ToUpper().Equals(nome.Trim().ToUpper()));        
            return cidade != null ? (int?)cidade.Id : null;            
        }

        public Cidade BuscarPorIdComIncludes(int id)
        {
            var result = DbSet
                        .Include(x => x.Estado).ThenInclude(x => x.Pais)
                        .FirstOrDefault(x => x.Id == id);

            return result;
        }

        public Cidade BuscarPorNomeComIncludes(string nmCidade)
        {
            var result = DbSet
                        .Include(x => x.Estado).ThenInclude(x => x.Pais)
                        .FirstOrDefault(x => x.NmCidade == nmCidade);

            return result;
        }

        public FiltroGenericoDtoBase<CidadeDto> FiltrarCidades(FiltroGenericoDtoBase<CidadeDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var query = DbSet.AsQueryable().AsNoTracking();

            query = query.Include(x => x.Estado).ThenInclude(x => x.Pais);

            query = query.Where(x => x.Id.ToString().ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper())
                                  || x.NmCidade.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                  || x.Estado.NmEstado.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                  || x.Estado.Pais.NmPais.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));

            var dados = query.Select(x => new CidadeDto
            {
                Id = x.Id,
                IdEstado = x.IdEstado,
                IdPais = x.Estado.IdPais,
                NmCidade = x.NmCidade,
                NmEstado = x.Estado.NmEstado,
                NmPais = x.Estado.Pais.NmPais
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
                dados = dados.OrderBy(x => x.NmCidade);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }
    }
}
