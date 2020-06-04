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
using static Cadastro.Domain.SharedRoot.SharedEnuns;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class TransferenciaPrestadorRepository : BaseRepository<TransferenciaPrestador>, ITransferenciaPrestadorRepository
    {
        public TransferenciaPrestadorRepository(CadastroContexto context, IVariablesToken variables,
            IAuditoriaRepository auditoriaRepository) : base(context, variables, auditoriaRepository)
        {
        }

        public FiltroComPeriodo<TransferenciaPrestadorDto> FiltrarTransferencias(FiltroComPeriodo<TransferenciaPrestadorDto> filtro)
        {
            var query = DbSet.AsQueryable().AsNoTracking();

            query = query.Where(x => x.DataTransferencia >= filtro.DataInicio && x.DataTransferencia <= filtro.DataFim);

            query = FiltrarCelulasComPermissao(filtro, query);

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

                query = query.Where(x => x.Prestador.Pessoa.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "finalizado" && x.Situacao ==
                                          SituacoesTransferenciaEnum.Efetivado.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "negado" &&
                                          x.Situacao == SituacoesTransferenciaEnum.Negado.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aguardando" && x.Situacao ==
                                          SituacoesTransferenciaEnum.AguardandoAprovacao.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aguardando aprovação" && x.Situacao ==
                                          SituacoesTransferenciaEnum.AguardandoAprovacao.GetHashCode()) ||
                                         (filtro.ValorParaFiltrar.ToLower() == "aprovado" && x.Situacao ==
                                          SituacoesTransferenciaEnum.Aprovado.GetHashCode()));
            }

            query = query.Include(x => x.Prestador);

            var dados = query.Select(x => new TransferenciaPrestadorDto
            {
                Id = x.Id,
                Celula = x.IdCelula.GetValueOrDefault(),
                Prestador = x.Prestador.Pessoa.Nome,
                Status = x.Situacao.ToString(),
                Usuario = x.Usuario,
                DataAlteracao = x.DataAlteracao
            });

            filtro.Total = dados.Count();

            switch (filtro?.CampoOrdenacao)
            {
                case "celula":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.Celula) : dados.OrderBy(x => x.Celula);
                    break;
                case "prestador":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.Prestador) : dados.OrderBy(x => x.Prestador);
                    break;
                case "status":
                    dados = filtro.OrdemOrdenacao.Equals("asc") ? dados.OrderByDescending(x => x.Status) : dados.OrderBy(x => x.Status);
                    break;
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();

            return filtro;
        }

        public IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComTransferenciaCadastrada()
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            var datas = DbSet.Where(x => x.DataTransferencia >= DateTime.Now.AddMonths(-5))
                .Select(d => new DateTime(d.DataTransferencia.Year, d.DataTransferencia.Month, 1))
                .Distinct()
                .ToList();

            var lista = new List<PeriodoTransferenciaPrestadorDto>();

            lista = datas.Select(x => new PeriodoTransferenciaPrestadorDto
            {
                NomePeriodo = x.ToString("MMMM/yyyy", culture),
                DataInicio = new DateTime(x.Year, x.Month, 1),
                DataFim = new DateTime(x.Year, x.Month, DateTime.DaysInMonth(x.Year, x.Month)),
            }).ToList();

            return lista;
        }

        public bool ExisteTransferenciaAberta(int idPrestador)
        {
            var existe = DbSet.Any(x => x.IdPrestador == idPrestador &&
                                        (x.Situacao == SharedEnuns.SituacoesTransferenciaEnum.AguardandoAprovacao.GetHashCode() ||
                                         x.Situacao == SharedEnuns.SituacoesTransferenciaEnum.Aprovado.GetHashCode()));

            return existe;
        }

        public PrestadorParaTransferenciaDto ConsultarTransferencia(int idTransferencia)
        {
            var transferencia = DbSet.FirstOrDefault(x => x.Id == idTransferencia);

            if (transferencia == null) return null;

            var result = new PrestadorParaTransferenciaDto
            {
                IdEmpresaGrupo = transferencia.IdEmpresaGrupo.GetValueOrDefault(),
                IdFilial = transferencia.IdFilial.GetValueOrDefault(),
                IdCelula = transferencia.IdCelula.GetValueOrDefault(),
                IdCliente = transferencia.IdCliente.GetValueOrDefault(),
                IdServico = transferencia.IdServico.GetValueOrDefault(),
                IdLocalTrabalho = transferencia.IdLocalTrabalho.GetValueOrDefault(),
                Id = transferencia.Id,
                IdPrestador = transferencia.IdPrestador,
                DataTransferencia = transferencia.DataTransferencia
            };

            return result;
        }

        private IQueryable<TransferenciaPrestador> FiltrarCelulasComPermissao(FiltroComPeriodo<TransferenciaPrestadorDto> filtro, IQueryable<TransferenciaPrestador> query)
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

            query = query.Where(x => celulasIds.Any(c => c == x.IdCelula.ToString()));
            return query;
        }
    }
}
