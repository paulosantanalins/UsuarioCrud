using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class FinalizacaoContratoRepository : BaseRepository<FinalizacaoContrato>, IFinalizacaoContratoRepository
    {
        public FinalizacaoContratoRepository(CadastroContexto context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

        public FiltroComPeriodo<FinalizarContratoGridDto> Filtrar(FiltroComPeriodo<FinalizarContratoGridDto> filtro)
        {
            if(!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();
            }
            
            var query = DbSet.AsQueryable().AsNoTracking();

            query = query.Where(x => x.DataFimContrato >= filtro.DataInicio && x.DataFimContrato <= filtro.DataFim);

            query = FiltrarCelulasComPermissao(filtro, query);

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                var parse = DateTime.TryParseExact(filtro.ValorParaFiltrar, "dd/MM/yyyy",
                    CultureInfo.CreateSpecificCulture("pt-BR"), DateTimeStyles.None, out var data);

                query = query.Where(x => x.Prestador.Pessoa.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "finalizado" && x.Situacao ==
                                          SharedEnuns.SituacoesFinalizarContrato.Finalizado.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "cancelado" && x.Situacao ==
                                          SharedEnuns.SituacoesFinalizarContrato.Cancelado.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "pendente" && x.Situacao ==
                                          SharedEnuns.SituacoesFinalizarContrato.Pendente.GetHashCode()) ||
                                         (parse && x.DataFimContrato.Date == data.Date));
            }

            query = query.Include(x => x.Prestador);

            switch (filtro.CampoOrdenacao)
            {
                case "idCelula":
                    query = filtro.OrdemOrdenacao.Equals("asc") ? query.OrderByDescending(x => x.Prestador.IdCelula) : query.OrderBy(x => x.Prestador.IdCelula);
                    break;
                case "prestador":
                    query = filtro.OrdemOrdenacao.Equals("asc") ? query.OrderByDescending(x => x.Prestador.Pessoa.Nome) : query.OrderBy(x => x.Prestador.Pessoa.Nome);
                    break;
                case "status":
                    query = filtro.OrdemOrdenacao.Equals("asc") ? query.OrderByDescending(x => x.Situacao) : query.OrderBy(x => x.Situacao);
                    break;
                case "diaFimContrato":
                    query = filtro.OrdemOrdenacao.Equals("asc") ? query.OrderByDescending(x => x.DataFimContrato) : query.OrderBy(x => x.DataFimContrato);
                    break;
            }

            var dados = query.Select(x => new FinalizarContratoGridDto
            {
                DiaFimContrato = x.DataFimContrato.ToString("dd/MM/yyyy"),
                Id = x.Id,
                IdCelula = x.Prestador.IdCelula,
                Prestador = x.Prestador.Pessoa.Nome,
                Status = x.Situacao,
                Usuario = x.Usuario,
                DataAlteracao = x.DataAlteracao
            });

            filtro.Total = dados.Count();

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        public IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastrada()
        {
            var culture = new CultureInfo("pt-BR");
            var datas = DbSet.Where(x => x.DataFimContrato >= DateTime.Now.AddMonths(-5))
                .Select(d => new DateTime(d.DataFimContrato.Year, d.DataFimContrato.Month, 1))
                .Distinct()
                .ToList();

            var lista = datas.Select(x => new PeriodoTransferenciaPrestadorDto
            {
                NomePeriodo = x.ToString("MMMM/yyyy", culture),
                DataInicio = new DateTime(x.Year, x.Month, 1),
                DataFim = new DateTime(x.Year, x.Month, DateTime.DaysInMonth(x.Year, x.Month))
            }).ToList();

            return lista;
        }

        public IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula, bool filtrar)
        {
            var query = _context.Prestador.Where(x => x.IdCelula == idCelula && !x.DataDesligamento.HasValue);

            query = filtrar
                ? query.Where(x => x.FinalizacoesContratos.All(y =>
                    y.Situacao != SharedEnuns.SituacoesFinalizarContrato.Pendente.GetHashCode()))
                : query;

            var result = query
                .Select(x => new KeyValueDto
                {
                    Id = x.Id,
                    Nome = x.Pessoa.Nome
                }).ToList();

            return result;
        }

        public FinalizacaoContratoDto ConsultarFinalizacao(int id)
        {
            var result = DbSet.Include(x => x.Prestador).AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (result == null) return null;

            var finalizarDto = new FinalizacaoContratoDto
            {
                DesabilitarContratosFuturos = !result.RetornoPermitido,
                FinalizarImediatamente = false,
                Id = result.Id,
                IdPrestador = result.IdPrestador,
                Motivo = result.Motivo,
                UltimoDiaTrabalho = result.DataFimContrato,
                IdCelula = result.Prestador.IdCelula
            };

            return finalizarDto;
        }

        public void AdicionarComLog(FinalizacaoContrato finalizacaoContrato)
        {
            var log = new LogFinalizacaoContrato
            {
                DataFimContrato = finalizacaoContrato.DataFimContrato,
                IdFinalizacaoContrato = finalizacaoContrato.Id,
                Motivo = finalizacaoContrato.Motivo,
                RetornoPermitido = finalizacaoContrato.RetornoPermitido,
                Situacao = finalizacaoContrato.Situacao,
                Acao = SharedEnuns.AcoesLog.NovaSolicitacao.GetHashCode()
            };

            finalizacaoContrato.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            log.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;

            if (finalizacaoContrato.DataAlteracao == null)
            {
                log.DataAlteracao = DateTime.Now;
                finalizacaoContrato.DataAlteracao = DateTime.Now;
            }

            finalizacaoContrato.LogsFinalizacaoCntrato.Add(log);

            DbSet.Add(finalizacaoContrato);
        }

        public void UpdateComLog(FinalizacaoContrato finalizacaoContrato)
        {
            var entityDB = DbSet.Find(finalizacaoContrato.Id);
            var log = new LogFinalizacaoContrato
            {
                DataFimContrato = finalizacaoContrato.DataFimContrato,
                IdFinalizacaoContrato = finalizacaoContrato.Id,
                Motivo = finalizacaoContrato.Motivo,
                RetornoPermitido = finalizacaoContrato.RetornoPermitido,
                Situacao = finalizacaoContrato.Situacao,
                Acao = SharedEnuns.AcoesLog.Edicao.GetHashCode()
            };

            _context.Entry(entityDB).CurrentValues.SetValues(finalizacaoContrato);

            entityDB.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            log.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            entityDB.DataAlteracao = DateTime.Now;
            log.DataAlteracao= DateTime.Now;

            DbSet.Update(entityDB);
            _context.Set<LogFinalizacaoContrato>().Add(log);
        }

        public void InativarFinalizacao(FinalizacaoContrato finalizacaoContrato, string motivo)
        {
            var entityDB = DbSet.Find(finalizacaoContrato.Id);
            var log = new LogFinalizacaoContrato
            {
                DataFimContrato = entityDB.DataFimContrato,
                IdFinalizacaoContrato = entityDB.Id,
                Motivo = entityDB.Motivo,
                RetornoPermitido = entityDB.RetornoPermitido,
                Situacao = entityDB.Situacao,
                Acao = SharedEnuns.AcoesLog.Inativacao.GetHashCode(),
                MotivoCancelamento = motivo
            };

            finalizacaoContrato.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            log.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            finalizacaoContrato.DataAlteracao = DateTime.Now;
            log.DataAlteracao = DateTime.Now;

            _context.Entry(finalizacaoContrato).Property(x => x.Situacao).IsModified = true;
            _context.Entry(finalizacaoContrato).Property(x => x.Usuario).IsModified = true;
            _context.Entry(finalizacaoContrato).Property(x => x.DataAlteracao).IsModified = true;
            _context.Set<LogFinalizacaoContrato>().Add(log);
        }

        public IEnumerable<LogFinalizacaoContrato> ObterLogsPorId(int id)
        {
            var logs = _context.Set<LogFinalizacaoContrato>().Where(x => x.IdFinalizacaoContrato == id).Select(x =>
                new LogFinalizacaoContrato
                {
                    Usuario = x.Usuario,
                    DataAlteracao = x.DataAlteracao,
                    Acao = x.Acao,
                    MotivoCancelamento = x.MotivoCancelamento
                });

            return logs.ToList();
        }

        public IEnumerable<FinalizacaoContrato> ObterFinalizacoesParaJob()
        {
            var finalizacoes = _context.Set<FinalizacaoContrato>().Where(x =>
                x.DataFimContrato <= DateTime.Now &&
                x.Situacao == SharedEnuns.SituacoesFinalizarContrato.Pendente.GetHashCode()).ToList();

            return finalizacoes;
        }

        private IQueryable<FinalizacaoContrato> FiltrarCelulasComPermissao(FiltroComPeriodo<FinalizarContratoGridDto> filtro, IQueryable<FinalizacaoContrato> query)
        {
            var celulasComAcesso = new List<string>();
            _variables.CelulasComPermissao.ForEach(x => celulasComAcesso.Add(x.ToString()));

            IEnumerable<string> celulasIds;
            if (string.IsNullOrEmpty(filtro.FiltroGenerico))
            {
                celulasIds = celulasComAcesso;
            }
            else
            {
                celulasIds = filtro.FiltroGenerico.Split(",");

                celulasIds = celulasIds.Intersect(celulasComAcesso).ToList();
            }

            query = query.Where(x => celulasIds.Any(c => c == x.Prestador.IdCelula.ToString()));
            return query;
        }
    }
}
