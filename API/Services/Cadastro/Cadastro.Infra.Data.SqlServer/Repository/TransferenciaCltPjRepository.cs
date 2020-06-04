using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Cadastro.Domain.SharedRoot;
using Utils;
using Utils.Base;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class TransferenciaCltPjRepository : BaseRepository<TransferenciaCltPj>, ITransferenciaCltPjRepository
    {
        public TransferenciaCltPjRepository(
            CadastroContexto context,
            IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {

        }

        public FiltroGenericoDtoBase<TransferenciaCltPjDto> Filtrar(FiltroGenericoDtoBase<TransferenciaCltPjDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                query = query.Where(x =>
                                       x.IdEacessoLegado.ToString().ToUpper().Equals(filtro.FiltroGenerico.ToUpper())
                                    || x.NomePrestador.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                    || x.IdPrestadorTransferido.ToString().ToUpper().Equals(filtro.FiltroGenerico.ToUpper()));
            }

            var dados = query.Select(p => new TransferenciaCltPjDto
            {
                Id = p.Id,
                IdEacessoLegado = p.IdEacessoLegado,
                IdPrestadorTransferido = p.IdPrestadorTransferido,
                NomePrestador = p.NomePrestador,
                DataAlteracao = p.DataAlteracao,
                Usuario = p.Usuario
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
                dados = dados.OrderByDescending(x => x.DataAlteracao).ThenBy(x => x.NomePrestador);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }
    }
}
