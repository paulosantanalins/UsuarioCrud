using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Cadastro.Infra.Data.SqlServer.Context;
using Dapper;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class ReajusteContratoRepository : BaseRepository<ReajusteContrato>, IReajusteContratoRepository
    {
        public ReajusteContratoRepository(CadastroContexto context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

        public FiltroComPeriodo<ReajusteContratoGridDto> Filtrar(FiltroComPeriodo<ReajusteContratoGridDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

            query = query.Where(x => x.DataReajuste >= filtro.DataInicio && x.DataReajuste <= filtro.DataFim && 
                                     x.Situacao != SharedEnuns.SituacoesReajusteContrato.ReajusteFinalizado.GetHashCode());

            query = FiltrarCelulasComPermissao(filtro, query);

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

                var parse = DateTime.TryParseExact(filtro.ValorParaFiltrar, "dd/MM/yyyy",
                    CultureInfo.CreateSpecificCulture("pt-BR"), DateTimeStyles.None, out var data);

                query = query.Where(x => x.Prestador.Pessoa.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aguardando aprovação bp" && x.Situacao ==
                                          SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aguardando aprovação remuneração" && x.Situacao ==
                                          SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoRemuneracao.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aguardando aprovação controladoria" && x.Situacao ==
                                             SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoContrtoladoria.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aguardando aprovação diretoria célula" && x.Situacao ==
                                             SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "reajuste aprovado" && x.Situacao ==
                                             SharedEnuns.SituacoesReajusteContrato.ReajusteAprovado.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "reajuste finalizado" && x.Situacao ==
                                             SharedEnuns.SituacoesReajusteContrato.ReajusteFinalizado.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "reajuste cancelado" && x.Situacao ==
                                             SharedEnuns.SituacoesReajusteContrato.ReajusteCancelado.GetHashCode()) ||
                                         (parse && x.DataInclusao.Date == data.Date) ||
                                         (parse && x.DataReajuste.Date == data.Date));
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
                case "dataSolicitacao":
                    query = filtro.OrdemOrdenacao.Equals("asc") ? query.OrderByDescending(x => x.DataInclusao) : query.OrderBy(x => x.DataInclusao);
                    break;
                case "dataReajuste":
                    query = filtro.OrdemOrdenacao.Equals("asc") ? query.OrderByDescending(x => x.DataReajuste) : query.OrderBy(x => x.DataReajuste);
                    break;
            }

            var dados = query.Select(x => new ReajusteContratoGridDto
            {
                Id = x.Id,
                IdCelula = x.Prestador.IdCelula,
                Prestador = x.Prestador.Pessoa.Nome,
                Status = x.Situacao,
                DataSolicitacao = x.DataInclusao.ToString("dd/MM/yyyy"),
                DataReajuste = x.DataReajuste.ToString("dd/MM/yyyy"),
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
            var datas = DbSet.Where(x => x.DataReajuste >= DateTime.Now.AddMonths(-5))
                .Select(d => new DateTime(d.DataReajuste.Year, d.DataReajuste.Month, 1))
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

        public IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula)
        {
            var query = _context.Prestador.Where(x => x.IdCelula == idCelula && !x.DataDesligamento.HasValue);

            var result = query
                .Select(x => new KeyValueDto
                {
                    Id = x.Id,
                    Nome = x.Pessoa.Nome
                }).ToList();

            return result;
        }

        public Prestador ObterPrestadorParaReajuste(int idPrestador, bool filtrar)
        {
            var query = _context.Prestador.Where(x => x.Id == idPrestador).AsNoTracking();

            query = filtrar
                ? query.Where(x => x.FinalizacoesContratos.All(y =>
                                       y.Situacao != SharedEnuns.SituacoesFinalizarContrato.Pendente.GetHashCode()) &&
                                   x.ReajustesContratos.All(y =>
                                       y.Situacao ==
                                       SharedEnuns.SituacoesReajusteContrato.ReajusteCancelado.GetHashCode() ||
                                       y.Situacao == SharedEnuns.SituacoesReajusteContrato.ReajusteFinalizado
                                           .GetHashCode()))
                : query;

            query = query.Include(x => x.ValoresPrestador).ThenInclude(x => x.TipoRemuneracao);

            return query.FirstOrDefault();
        }

        public IEnumerable<LogReajusteContrato> ObterLogsPorId(int id)
        {
            var logs = _context.Set<LogReajusteContrato>().Where(x => x.IdReajusteContrato == id).Select(x =>
                new LogReajusteContrato
                {
                    Usuario = x.Usuario,
                    DataAlteracao = x.DataAlteracao,
                    Acao = x.Acao,
                    Motivo = x.Motivo
                });

            return logs.ToList();
        }

        public void AdicionarComLog(ReajusteContrato reajusteContrato)
        {
            var log = new LogReajusteContrato
            {
                DataReajuste = reajusteContrato.DataReajuste,
                IdReajusteContrato = reajusteContrato.Id,
                IdTipoContrato = reajusteContrato.IdTipoContrato,
                QuantidadeHorasContrato = reajusteContrato.QuantidadeHorasContrato,
                ValorContrato = reajusteContrato.ValorContrato,
                DataInclusao = reajusteContrato.DataInclusao,
                Situacao = reajusteContrato.Situacao,
                Acao = SharedEnuns.AcoesLog.NovaSolicitacao.GetHashCode()
            };

            reajusteContrato.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            log.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;

            if (reajusteContrato.DataAlteracao == null)
            {
                log.DataAlteracao = DateTime.Now;
                reajusteContrato.DataAlteracao = DateTime.Now;
            }

            reajusteContrato.LogsReajusteContratos.Add(log);

            DbSet.Add(reajusteContrato);
        }

        public void UpdateComLog(ReajusteContrato reajusteContrato, int acao)
        {
            var entityDB = DbSet.Find(reajusteContrato.Id);
            var log = new LogReajusteContrato
            {
                DataReajuste = reajusteContrato.DataReajuste,
                IdReajusteContrato = reajusteContrato.Id,
                IdTipoContrato = reajusteContrato.IdTipoContrato,
                QuantidadeHorasContrato = reajusteContrato.QuantidadeHorasContrato,
                ValorContrato = reajusteContrato.ValorContrato,
                DataInclusao = reajusteContrato.DataInclusao,
                Situacao = reajusteContrato.Situacao,
                Acao = acao
            };

            _context.Entry(entityDB).CurrentValues.SetValues(reajusteContrato);

            entityDB.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            log.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            entityDB.DataAlteracao = DateTime.Now;
            log.DataAlteracao = DateTime.Now;

            DbSet.Update(entityDB);
            _context.Set<LogReajusteContrato>().Add(log);
        }

        public void InativarFinalizacao(ReajusteContrato reajusteContrato, string motivo)
        {
            var entityDB = DbSet.Find(reajusteContrato.Id);
            var log = new LogReajusteContrato
            {
                DataReajuste = entityDB.DataReajuste,
                IdReajusteContrato = entityDB.Id,
                IdTipoContrato = entityDB.IdTipoContrato,
                QuantidadeHorasContrato = entityDB.QuantidadeHorasContrato,
                ValorContrato = entityDB.ValorContrato,
                DataInclusao = entityDB.DataInclusao,
                Situacao = entityDB.Situacao,
                Motivo = motivo,
                Acao = SharedEnuns.AcoesLog.Negado.GetHashCode()
            };

            reajusteContrato.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            log.Usuario = string.IsNullOrEmpty(_variables.UserName) ? "STFCORP" : _variables.UserName;
            reajusteContrato.DataAlteracao = DateTime.Now;
            log.DataAlteracao = DateTime.Now;

            _context.Entry(reajusteContrato).Property(x => x.Situacao).IsModified = true;
            _context.Entry(reajusteContrato).Property(x => x.Usuario).IsModified = true;
            _context.Entry(reajusteContrato).Property(x => x.DataAlteracao).IsModified = true;
            _context.Set<LogReajusteContrato>().Add(log);
        }

        public ValoresContratoPrestadorModel ConsultarReajuste(int id)
        {
            var result = DbSet.Include(x => x.Prestador).AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (result == null) return null;

            var finalizarDto = new ValoresContratoPrestadorModel
            {
                Id = result.Id,
                IdPrestador = result.IdPrestador,
                IdTipoContrato = result.IdTipoContrato,
                QuantidadeNova = result.QuantidadeHorasContrato,
                ValorNovo = result.ValorContrato,
                DataReajuste = result.DataReajuste,
                IdCelula = result.Prestador.IdCelula,
                Situacao = result.Situacao
            };

            return finalizarDto;
        }

        public IEnumerable<ReajusteContrato> ObterReajustesParaJob()
        {
            var reajustes = _context.Set<ReajusteContrato>().Where(x =>
                x.DataReajuste <= DateTime.Now &&
                x.Situacao == SharedEnuns.SituacoesReajusteContrato.ReajusteAprovado.GetHashCode()).ToList();

            return reajustes;
        }

        public int ObterIdFuncionalidade(string nomeFuncionalidade)
        {
            var query = $@"SELECT IDFUNCIONALIDADE as Id
                          FROM dbo.TBLFUNCIONALIDADE
                          WHERE NMFUNCIONALIDADE = '{nomeFuncionalidade}'";

            var result = _context.Database.GetDbConnection().QueryFirst<int>(query);
            _context.Database.GetDbConnection().Close();

            return result;
        }

        public IEnumerable<string> ObterLoginsComFuncionalidade(int idFuncionalidade, Prestador prestador)
        {
            var query = $@"SELECT DISTINCT usuarioPerfil.LGUSUARIOLOGADO
                                            FROM dbo.TBLFUNCIONALIDADE func
                                            JOIN dbo.TBLVINCULOPERFILFUNCIONALIDADE perfilfunc ON perfilfunc.IDFUNCIONALIDADE = func.IDFUNCIONALIDADE
                                            JOIN dbo.tblUsuarioPerfil usuarioPerfil ON usuarioPerfil.IDPERFIL = perfilfunc.IDPERFIL
                                            JOIN dbo.TBLVISUALIZACAOCELULA visualizacelula ON visualizacelula.LGUSUARIOVINCULADO = usuarioPerfil.LGUSUARIOLOGADO
                                            WHERE func.IDFUNCIONALIDADE = {idFuncionalidade}
                                              AND visualizacelula.IDCELULA = {prestador.IdCelula} ";

            var result = _context.Database.GetDbConnection().Query<string>(query);
            _context.Database.GetDbConnection().Close();

            return result;
        }

        public IEnumerable<string> ObterEmailGerenteCelula(int idFuncionalidade, Prestador prestador)
        {
            var query = $@"SELECT DISTINCT celula.NMEMAILRESPONSAVEL
                                           FROM dbo.TBLFUNCIONALIDADE func
                                           JOIN dbo.TBLVINCULOPERFILFUNCIONALIDADE perfilfunc ON perfilfunc.IDFUNCIONALIDADE = func.IDFUNCIONALIDADE
                                           JOIN dbo.tblUsuarioPerfil usuarioPerfil ON usuarioPerfil.IDPERFIL = perfilfunc.IDPERFIL
                                           JOIN dbo.TBLVISUALIZACAOCELULA visualizacelula ON visualizacelula.LGUSUARIOVINCULADO = usuarioPerfil.LGUSUARIOLOGADO
                                           JOIN dbo.TBLCELULA celula ON celula.IDCELULA = visualizacelula.IDCELULA
                                           WHERE func.IDFUNCIONALIDADE = {idFuncionalidade}
                                             AND visualizacelula.IDCELULA = {prestador.IdCelula} ";

            var result = _context.Database.GetDbConnection().Query<string>(query);
            _context.Database.GetDbConnection().Close();

            return result;
        }

        private IQueryable<ReajusteContrato> FiltrarCelulasComPermissao(FiltroComPeriodo<ReajusteContratoGridDto> filtro, IQueryable<ReajusteContrato> query)
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
