using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Extensions;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class DescontoPrestadorRepository : BaseRepository<DescontoPrestador>, IDescontoPrestadorRepository
    {
        public DescontoPrestadorRepository(CadastroContexto context, IVariablesToken variables, IAuditoriaRepository auditoriaRepository)
            : base(context, variables, auditoriaRepository)
        {

        }


        public FiltroGenericoDtoBase<DescontoPrestadorDto> Filtrar(FiltroGenericoDtoBase<DescontoPrestadorDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var query = DbSet.AsQueryable().AsNoTracking();
            query = query.Include(x => x.HorasMesPrestador).ThenInclude(y => y.Prestador)
                         .Include(x => x.HorasMesPrestador.LogsHorasMesPrestador)
                         .Include(x => x.Desconto);

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.HorasMesPrestador.Prestador.IdCelula));

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x =>
                                     x.HorasMesPrestador.Prestador.Pessoa.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.HorasMesPrestador.Prestador.IdCelula.ToString().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.Desconto.DescricaoValor.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.ValorDesconto.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            }

            if (filtro.FiltroGenerico != null && filtro.FiltroGenerico.Any())
            {
                int idHorasMes = Convert.ToInt32(filtro.FiltroGenerico);

                query = query.Where(x => x.HorasMesPrestador.IdHorasMes == idHorasMes);
            }

            var dados = query.Select(p => new DescontoPrestadorDto
            {
                Id = p.Id,
                IdCelula = p.HorasMesPrestador.Prestador.IdCelula,
                NomePrestador = p.HorasMesPrestador.Prestador.Pessoa.Nome,
                TipoDesconto = p.Desconto.DescricaoValor,
                ValorDesconto = p.ValorDesconto,
                Observacao = p.DescricaoDesconto,
                IdHorasMesPrestador = p.IdHorasMesPrestador,
                IdDesconto = p.IdDesconto,                
                EnviadoParaRM = p.HorasMesPrestador.Situacao == SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_INTEGRACAO.GetDescription() ? true : false,
                DataAlteracao = p.DataAlteracao.Value,
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
                dados = dados.OrderBy(x => x.IdHorasMesPrestador).ThenBy(y => y.Id);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).OrderBy(x => x.IdCelula).ToList();

            return filtro;
        }


        public decimal ObterValorDescontoPorIdHorasMesPrestador(int idHorasMesPrestador, string tipoDesconto)
        {
            var query = DbSet.AsNoTracking()
                .Include(x => x.HorasMesPrestador)
                .ThenInclude(y => y.Prestador)
                .Where(x => x.HorasMesPrestador.Id.Equals(idHorasMesPrestador) 
                         && x.Desconto.DescricaoValor.Equals(tipoDesconto));

            var soma = query.Sum(x => x.ValorDesconto);
            return soma;
        }
    }
}
