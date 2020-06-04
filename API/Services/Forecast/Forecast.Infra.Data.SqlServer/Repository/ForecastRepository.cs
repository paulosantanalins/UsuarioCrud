using Forecast.Domain.ForecastRoot;
using Forecast.Domain.ForecastRoot.Dto;
using Forecast.Domain.ForecastRoot.Repository;
using Forecast.Domain.SharedRoot;
using Forecast.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Utils.Base;

namespace Forecast.Infra.Data.SqlServer.Repository
{
    public class ForecastRepository : IForecastRepository
    {

        protected DbSet<ForecastET> DbSet;
        private readonly ForecastContext _context;
        private readonly IVariablesToken _variables;

        public ForecastRepository(ForecastContext context, IVariablesToken variables)
        {
            _context = context;
            _variables = variables;
            DbSet = _context.Set<ForecastET>();
        }

        public FiltroGenericoDtoBase<ForecastDto> Filtrar(FiltroGenericoDtoBase<ForecastDto> filtro, List<int> idClientes, List<int> idServicos)
        {
            var query = DbSet
                        .Include(x => x.ValorForecast)
                        .AsQueryable().AsNoTracking();

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelula));

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar) && filtro.ValorParaFiltrar.Any())
            {
                var valorParaFiltrarSplit = filtro.ValorParaFiltrar.Split(',');

                if (valorParaFiltrarSplit != null && valorParaFiltrarSplit.Length > 0)
                {
                    var idCelula = int.Parse(valorParaFiltrarSplit[0]);
                    if (idCelula > 0)
                    {
                        query = query.Where(x => x.IdCelula.ToString().ToUpper().Equals(valorParaFiltrarSplit[0]));
                    }

                    var idCliente = int.Parse(valorParaFiltrarSplit[1]);
                    if (idCliente > 0)
                    {
                        query = query.Where(x => x.IdCliente.ToString().ToUpper().Equals(valorParaFiltrarSplit[1]));
                    }

                    var idServico = int.Parse(valorParaFiltrarSplit[2]);
                    if (idServico > 0)
                    {
                        query = query.Where(x => x.IdServico.ToString().ToUpper().Equals(valorParaFiltrarSplit[2]));
                    }

                    var idAno = int.Parse(valorParaFiltrarSplit[3]);
                    if (idAno > 0)
                    {
                        query = query.Where(x => x.NrAno.ToString().ToUpper().Equals(valorParaFiltrarSplit[3]));
                    }

                }
            }

            var dados = query.Select(f => new ForecastDto
            {
                IdCelula = f.IdCelula,
                IdCliente = f.IdCliente,
                IdServico = f.IdServico,
                NrAno = f.NrAno,
                DataAniversario = f.DataAniversario,
                DataAplicacaoReajuste = f.DataAplicacaoReajuste,
                DataReajusteRetroativo = f.DataReajusteRetroativo,
                VlPercentual = f.ValorForecast.VlPercentual,
                IdStatus = f.IdStatus,
                DescricaoJustificativa = f.DescricaoJustificativa,
                ValorJaneiro = f.ValorForecast.ValorJaneiro,
                ValorFevereiro = f.ValorForecast.ValorFevereiro,
                ValorMarco = f.ValorForecast.ValorMarco,
                ValorAbril = f.ValorForecast.ValorAbril,
                ValorMaio = f.ValorForecast.ValorMaio,
                ValorJunho = f.ValorForecast.ValorJunho,
                ValorJulho = f.ValorForecast.ValorJulho,
                ValorAgosto = f.ValorForecast.ValorAgosto,
                ValorSetembro = f.ValorForecast.ValorSetembro,
                ValorOutubro = f.ValorForecast.ValorOutubro,
                ValorNovembro = f.ValorForecast.ValorNovembro,
                ValorDezembro = f.ValorForecast.ValorDezembro,
                ValorTotal = obterValorTotal(f),
                DataAlteracao = f.DataAlteracao,
                Usuario = f.Usuario,
                FaturamentoNaoRecorrente = f.FaturamentoNaoRecorrente,  
            });

            filtro.Total = dados.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);

            filtro.Valores = dados.ToList();

            return filtro;
        }
        
        public bool? DefinirAlertaAniversarioForecast(ForecastDto forecast)
        {            
            var dataInicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);

            var ultimoRegistroForecastPorAno =  DbSet.Where(x => x.IdCelula == forecast.IdCelula && x.IdCliente == forecast.IdCliente && x.IdServico == forecast.IdServico)
                    .OrderBy(x => x.NrAno)
                    .LastOrDefault();

            return !ultimoRegistroForecastPorAno.DataAniversario.HasValue ? (bool?)null : ((ultimoRegistroForecastPorAno.DataAniversario >= dataInicioMes) ? true : false);
        }

        private Decimal? obterValorTotal(ForecastET forecastET)
        {
            return forecastET.ValorForecast.ValorJaneiro + forecastET.ValorForecast.ValorFevereiro + forecastET.ValorForecast.ValorMarco
                                     + forecastET.ValorForecast.ValorAbril + forecastET.ValorForecast.ValorMaio + forecastET.ValorForecast.ValorJunho
                                     + forecastET.ValorForecast.ValorJulho + forecastET.ValorForecast.ValorAgosto + forecastET.ValorForecast.ValorSetembro
                                     + forecastET.ValorForecast.ValorOutubro + forecastET.ValorForecast.ValorNovembro + forecastET.ValorForecast.ValorDezembro;
        }

        public ForecastET Buscar(int idCelula, int idCliente, int idServico, int ano)
        {
            return DbSet.FirstOrDefault(x => x.IdCelula == idCelula && x.IdCliente == idCliente && x.IdServico == idServico && x.NrAno == ano);
        }

        public ForecastET BuscarPorIdComIncludes(int idCelula, int idCliente, int idServico, int nrAno)
        {
            var result = DbSet
                        .Include(x => x.ValorForecast)
                        .Where(x => x.IdCelula == idCelula
                                    && x.IdCliente == idCliente
                                    && x.IdServico == idServico
                                    && x.NrAno == nrAno)
                        .FirstOrDefault();

            return result;
        }

        public List<ForecastET> BuscarTodosPorComIncludes()
        {
            var result = DbSet
                        .Include(x => x.ValorForecast).ToList();

            return result;
        }

        public void Adicionar(ForecastET entity)
        {
            entity.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;

            if (entity.DataAlteracao == null)
            {
                entity.DataAlteracao = DateTime.Now;
            }
            DbSet.Add(entity);
        }

        public void Update(ForecastET forecast)
        {
            _context.Entry(forecast);
            DbSet.Update(forecast);
            AtualizarDataAlteracaoEUsuario();
        }

        private void AtualizarDataAlteracaoEUsuario()
        {
            var trackedEntities = _context.ChangeTracker.Entries<EntityBaseCompose>();
            foreach (var entity in trackedEntities)
            {
                entity.Entity.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
                entity.Entity.DataAlteracao = DateTime.Now;
            }
        }

        public ICollection<ForecastET> BuscarTodos()
        {
            return DbSet.ToList();
        }

        public ForecastET BuscarPorId(int id)
        {
            return DbSet.Find(id);
        }

        public void AdicionarRange(List<ForecastET> entities)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            if (string.IsNullOrEmpty(_variables.UserName))
            {
                _variables.UserName = "Eacesso";
            }

            foreach (var entity in entities)
            {
                entity.Usuario = _variables.UserName;
                if (entity.DataAlteracao == null)
                {
                    entity.DataAlteracao = DateTime.Now;
                }
                DbSet.Add(entity);
            }
        }
    }
}
