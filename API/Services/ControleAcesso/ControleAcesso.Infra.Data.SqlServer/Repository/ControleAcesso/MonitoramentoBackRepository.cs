using ControleAcesso.Domain.MonitoramentoRoot.DTO;
using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using ControleAcesso.Domain.MonitoramentoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Context;
using ControleAcesso.Infra.Data.SqlServer.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ControleAcesso.Domain.SharedRoot;
using Utils;

namespace ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso
{
    public class MonitoramentoBackRepository : BaseRepository<MonitoramentoBack>, IMonitoramentoBackRepository
    {
        public MonitoramentoBackRepository(ControleAcessoContext context, IVariablesToken variables) : base(context, variables)
        {

        }

        public FiltroGenericoDto<MonitoramentoBackDto> FiltrarBackend(FiltroGenericoDto<MonitoramentoBackDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar) && filtro.ValorParaFiltrar.Any())
            {
                query = query.Where(x =>
                                       x.TipoLog.ToString().ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.Origem.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.DetalheLog.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.StackTrace.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()));
            }

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                query = query.Where(x => x.Origem.Equals(filtro.FiltroGenerico, StringComparison.InvariantCultureIgnoreCase));
            }

            var dados = query.Select(p => new MonitoramentoBackDto
            {
                Id = p.Id,
                TipoLog = p.TipoLog,
                Origem = p.Origem,
                DetalheLog = p.DetalheLog,
                StackTrace = p.StackTrace,
                DataAlteracao = p.DataAlteracao
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
                dados = dados.OrderByDescending(x => x.DataAlteracao).ThenBy(x => x.Origem).ThenBy(y => y.TipoLog);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }
    }
}
