using AutoMapper;
using Cadastro.Domain.CidadeRoot.Repository;
using Cadastro.Domain.DominioRoot.Repository;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.PessoaRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.RmRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.TelefoneRoot.Entity;
using Cadastro.Infra.Data.SqlServer.Context;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Dapper;
using Logger.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.Extensions;

namespace Cadastro.Infra.Data.SqlServer.Repository
{
    public class PrestadorRepository : BaseRepository<Prestador>, IPrestadorRepository
    {
        private readonly ICidadeRepository _cidadeRepository;
        private readonly ICelulaRepository _celulaRepository;
        private readonly IRmService _rmService;
        private readonly IHorasMesPrestadorRepository _horasMesPrestadorRepository;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IDominioRepository _dominioRepository;
        private readonly IHorasMesRepository _horasMesRepository;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        const int APROVAR_PAGAMENTOS = 0;

        public PrestadorRepository(CadastroContexto context,
            IVariablesToken variables,
            ICidadeRepository cidadeRepository,
            ICelulaRepository celulaRepository,
            IRmService rmService,
            IHorasMesPrestadorRepository horasMesPrestadorRepository,
            IPessoaRepository pessoaRepository,
            IAuditoriaRepository auditoriaRepository,
            IDominioRepository dominioRepository,
            IHorasMesRepository horasMesRepository,
            MicroServicosUrls microServicosUrls,
            IOptions<ConnectionStrings> connectionStrings)
            : base(context, variables, auditoriaRepository)
        {
            _cidadeRepository = cidadeRepository;
            _rmService = rmService;
            _celulaRepository = celulaRepository;
            _pessoaRepository = pessoaRepository;
            _microServicosUrls = microServicosUrls;
            _connectionStrings = connectionStrings;
            _horasMesPrestadorRepository = horasMesPrestadorRepository;
            _dominioRepository = dominioRepository;
            _horasMesRepository = horasMesRepository;
        }

        public Prestador BuscarPorIdComIncludes(int id)
        {                   
            var result = DbSet
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.Endereco)
                                .ThenInclude(x => x.Cidade)
                                    .ThenInclude(x => x.Estado)
                                        .ThenInclude(x => x.Pais)
                        .Include(x => x.Pessoa)
                           .ThenInclude(x => x.Endereco)
                              .ThenInclude(x => x.AbreviaturaLogradouro)
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.Telefone)
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.Nacionalidade)
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.Escolaridade)
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.Extensao)
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.EstadoCivil)
                        .Include(x => x.Pessoa)
                            .ThenInclude(x => x.Graduacao)
                        .Include(x => x.Pessoa)
                          .ThenInclude(x => x.Sexo)
                        .Include(x => x.Cargo)
                        .Include(x => x.Contratacao)                     
                        .Include(x => x.SituacaoPrestador)
                        .Include(x => x.DiaPagamento)
                        .Include(x => x.AreaFormacao)
                        .Include(x => x.InativacoesPrestador)
                        .Include(x => x.ValoresPrestador)
                        .Include(x => x.ObservacoesPrestador).ThenInclude(o => o.TipoOcorrencia)
                        .Include(x => x.ClientesServicosPrestador)
                        .Include(x => x.ContratosPrestador)
                            .ThenInclude(x => x.ExtensoesContratoPrestador)
                        .Include(x => x.DocumentosPrestador)
                            .ThenInclude(x => x.TipoDocumentoPrestador)
                        .Include(x => x.EmpresasPrestador)
                            .ThenInclude(x => x.Empresa)
                                .ThenInclude(x => x.Endereco)
                                    .ThenInclude(x => x.AbreviaturaLogradouro)
                        .Include(x => x.EmpresasPrestador)
                            .ThenInclude(x => x.Empresa)
                                .ThenInclude(x => x.Endereco)
                                    .ThenInclude(x => x.Cidade)
                                        .ThenInclude(x => x.Estado)
                                            .ThenInclude(x => x.Pais)                       
                        .FirstOrDefault(x => x.Id == id);

            result.ValoresPrestador = result.ValoresPrestador.OrderByDescending(x => x.DataAlteracao).ToList();
            result.InativacoesPrestador = result.InativacoesPrestador.OrderByDescending(x => x.DataDesligamento).ToList();
            result.ClientesServicosPrestador = result.ClientesServicosPrestador.OrderByDescending(x => x.DataInicio).ToList();
            result.EmpresasPrestador = result.EmpresasPrestador.OrderByDescending(x => x.DataAlteracao).ToList();
            result.ObservacoesPrestador = result.ObservacoesPrestador.OrderByDescending(x => x.DataAlteracao).ToList();
            result.DocumentosPrestador = result.DocumentosPrestador.Where(x => !x.Inativo)?.ToList() ?? null;
            result.ContratosPrestador = result.ContratosPrestador?.Where(x => !x.Inativo)
                ?.Select(contrato =>
                {
                    contrato.ExtensoesContratoPrestador = contrato.ExtensoesContratoPrestador?.Where(extensao => !extensao.Inativo)?.ToList() ?? null;
                    return contrato;
                }).ToList() ?? null;

            return result;
        }

        public Prestador BuscarPorIdComIncludeCelula(int id)
        {
            var result = DbSet
                        .Include(x => x.Pessoa)
                        .Include(x => x.Celula)
                            .ThenInclude(x => x.CelulaSuperior)
                        .Include(x => x.Celula)
                            .ThenInclude(x => x.Pessoa)
                        .FirstOrDefault(x => x.Id == id);
            return result;
        }

        public IQueryable<Prestador> BuscarPorIdCelula(int id, int idHorasMes)
        {
            var result = DbSet
                      .Include(x => x.Pessoa)
                        .Include(x => x.HorasMesPrestador)
                        .AsQueryable()
                        .Where(x => x.IdCelula == id && x.HorasMesPrestador.Any(y => y.IdHorasMes == idHorasMes));


            return result;
        }

        public FiltroGenericoDtoBase<PrestadorDto> Filtrar(FiltroGenericoDtoBase<PrestadorDto> filtro)
        {
            const int TODOS_STATUS = -1;
            const int ATIVOS = 1;
            const int INATIVOS = 0;

            var query = DbSet.AsQueryable().AsNoTracking();
            query = query
                .Include(x => x.Pessoa)
                .Include(x => x.Cargo)
                .Include(x => x.Contratacao);

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelula));

            if (filtro.Id != TODOS_STATUS)
            {
                query = query.Where(x => filtro.Id == ATIVOS ? !x.DataDesligamento.HasValue : x.DataDesligamento.HasValue);
            }

            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar) && filtro.ValorParaFiltrar.Any())
            {
                List<int> valorParaFiltrarList = new List<int>();

                var valorParaFiltrarSplit = filtro.ValorParaFiltrar.Split(',');
                for (int i = 0; i < valorParaFiltrarSplit.Length; i++)
                {
                    valorParaFiltrarList.Add(int.Parse(valorParaFiltrarSplit[i]));
                }

                query = query.Where(x => valorParaFiltrarList.Any(y => y == x.IdCelula));
            }

            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                filtro.FiltroGenerico = filtro.FiltroGenerico.Trim();

                query = query.Where(x =>
                                       x.IdCelula.ToString().ToUpper().Equals(filtro.FiltroGenerico.ToUpper())
                                    || x.Pessoa.Nome.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                    || x.Cargo.DescricaoValor.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                    || x.Contratacao.DescricaoValor.ToUpper().Contains(filtro.FiltroGenerico.ToUpper())
                                    || (x.Pessoa.Cpf != null ? x.Pessoa.Cpf.ToUpper().Equals(filtro.FiltroGenerico.Replace(".", "").Replace("-", "").ToUpper()) : false)
                                    || (filtro.FiltroGenerico.ToUpper().Equals(x.DataDesligamento.HasValue ? "INATIVO" : "ATIVO")));
            }

            var dados = query.Select(p => new PrestadorDto
            {
                Id = p.Id,
                IdCelula = p.IdCelula,
                Nome = p.Pessoa.Nome,
                Funcao = p.Cargo.DescricaoValor,
                Contratacao = p.Contratacao.DescricaoValor,
                CPF = p.Pessoa.Cpf,
                Status = p.DataDesligamento.HasValue ? "Inativo" : "Ativo",
                DiaPagamento = Convert.ToInt32(p.DiaPagamento.DescricaoValor),
                Remuneracao = p.IdTipoRemuneracao.HasValue ? p.TipoRemuneracao.DescricaoValor : "",
                DataAlteracao = p.DataAlteracao,
                DataDesligamento = p.DataDesligamento,
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
                dados = dados.OrderBy(x => x.Status).ThenBy(x => x.IdCelula).ThenBy(y => y.Nome);
            }

            filtro.Valores = dados.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        public FiltroGenericoDtoBase<PrestadorDto> FiltrarHoras(FiltroGenericoDtoBase<PrestadorDto> filtro)
        {
            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var mesAtual = DateTime.Now;
            var mesAntigo = DateTime.Now.AddMonths(-1);

            var query = DbSet.AsQueryable();
            query = query            
                .Include(x => x.Pessoa)
                .Include(x => x.Cargo)
                .Include(x => x.Contratacao)
                .Include(x => x.DiaPagamento)
                .Include(x => x.TipoRemuneracao)
                .Include(x => x.ValoresPrestador)
                .Include(x => x.HorasMesPrestador).ThenInclude(x => x.HorasMes)
                .Where(x => x.IdDiaPagamento.HasValue && x.IdTipoRemuneracao.HasValue && x.HorasMesPrestador.Any() &&
                            (!x.DataDesligamento.HasValue || (x.DataDesligamento.Value.Month == mesAtual.Month && x.DataDesligamento.Value.Year == mesAtual.Year)
                                || (x.DataDesligamento.Value.Month == mesAntigo.Month && x.DataDesligamento.Value.Year == mesAntigo.Year)));

            int idHorasMes = 0;
            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                idHorasMes = Convert.ToInt32(filtro.FiltroGenerico);
            }
            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelula));

            var horasMes = _horasMesRepository.BuscarPorId(idHorasMes);

            var dados = query.ToList();

            var lista = dados.Select(p => new PrestadorDto
            {
                Id = p.Id,
                IdCelula = p.IdCelula,
                Nome = p.Pessoa.Nome,
                Funcao = p.Cargo.DescricaoValor,
                Contratacao = p.Contratacao.DescricaoValor,
                CPF = p.Pessoa.Cpf,
                Status = p.DataDesligamento.HasValue ? "Inativo" : "Ativo",
                DiaPagamento = Convert.ToInt32(p.DiaPagamento.DescricaoValor),
                Remuneracao = (p.IdTipoRemuneracao.HasValue && p.IdTipoRemuneracao != 0) ? p.TipoRemuneracao.DescricaoValor : "",
                DataAdmissao = p.DataInicio,
                DataDesligamento = p.DataDesligamento,
                TipoAprovacao = ObterTipoAprovacao(p, idHorasMes),
                Horas = ObterHoras(p, idHorasMes, true),
                Extras = ObterHoras(p, idHorasMes, false),
                Situacao = ObterSituacao(p, idHorasMes),
                Total = ObterValor(p, idHorasMes, false, horasMes),
                DataAlteracao = p.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes) != null ?
                                  p.HorasMesPrestador.First(x => x.IdHorasMes == idHorasMes).DataAlteracao
                                  : p.HorasMesPrestador.Last().DataAlteracao,
                ObservacaoSemPrestacaoServico = ObservacaoSemPrestacaoServico(p, idHorasMes),
                Usuario = p.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes) != null ?
                            p.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes).Usuario 
                            : p.HorasMesPrestador.Last().Usuario,
                ValorMes = p.TipoRemuneracao.DescricaoValor == "MENSALISTA" ? (p.ValoresPrestador.FirstOrDefault(x => x.IdPrestador == p.Id) != null ?
                            p.ValoresPrestador.FirstOrDefault(x => x.IdPrestador == p.Id).ValorMes : 0).ToString() : "N/A",
                ValorHora = p.ValoresPrestador.FirstOrDefault(x => x.IdPrestador == p.Id) != null ?
                            p.ValoresPrestador.FirstOrDefault(x => x.IdPrestador == p.Id).ValorHora : 0
            });

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                lista = lista.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                lista = lista.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else
            {
                lista = lista.OrderBy(y => y.IdCelula).ThenBy(y => y.Nome).ToList();
            }

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                lista = lista.Where(x => x.IdCelula.ToString().Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || x.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || (x.Remuneracao != null ? x.Remuneracao.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper()) : false)
                                    || (x.DiaPagamento.HasValue ? x.DiaPagamento.ToString().Equals(filtro.ValorParaFiltrar.ToUpper()) : false)
                                    || (x.Horas.HasValue ? x.Horas.Value.Equals(filtro.ValorParaFiltrar.ToUpper()) : false)
                                    || (x.Extras.HasValue ? x.Extras.Value.Equals(filtro.ValorParaFiltrar.ToUpper()) : false)
                                    || x.Situacao.ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || x.TipoAprovacao.ToUpper().Equals(filtro.ValorParaFiltrar.ToUpper())
                                    ).ToList();
            }

            filtro.Total = lista.Count();

            filtro.Valores = lista.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        private decimal? ObterHorasTotal(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                var horas = horasMes.Horas ?? 0;
                var extras = horasMes.Extras ?? 0;
                return horas + extras;
            }
            else
            {
                return null;
            }
        }

        private string ObservacaoSemPrestacaoServico(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                return horasMes.ObservacaoSemPrestacaoServico;
            }
            return String.Empty;
        }

        private decimal? ObterHoras(Prestador prestador, int idHorasMes, bool horas)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horas)
                {
                    return horasMes.Horas;
                }
                else
                {
                    return horasMes.Extras;
                }
            }
            else
            {
                return null;
            }
        }

        private int? ObterChaveOrigemRm(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                return horasMes.IdChaveOrigemIntRm;
            }
            else
            {
                return null;
            }
        }

        private string ObterSituacao(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                return horasMes.Situacao == null ? "" : horasMes.Situacao;
            }
            else
            {
                return SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription();
            }
        }

        private int? ObterIdHoraMesPrestador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                return horasMes.Id;
            }
            return null;
        }

        private bool? ObterAprovar(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                return VerificaStatusAprovado(horasMes.Situacao.ToUpper());
            }
            return null;
        }

        private bool? VerificaStatusAprovado(string situacaoAtual)
        {
            if (situacaoAtual.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.NEGADO.GetDescription()))
            {
                return false;
            }
            else if (
                situacaoAtual.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription()) ||
                situacaoAtual.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_CADASTRADAS.GetDescription()) ||
                situacaoAtual.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_RECADASTRADAS.GetDescription()) ||
                situacaoAtual.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription()))
            {
                return null;
            }
            else
            {
                return true;
            }
        }

        private string ObterTipoAprovacao(Prestador prestador, int idPeriodo)
        {
            var periodo = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idPeriodo);
            if (periodo != null)
            {
                if (periodo.SemPrestacaoServico)
                {
                    return "N/A";
                }
                else
                {
                    var horasPeriodo = periodo.HorasMes.Horas;
                    if (periodo.Extras.HasValue)
                    {
                        return "ADICIONAL";
                    }
                    else if (periodo.Horas == horasPeriodo)
                    {
                        return "TOTAL";
                    }
                    else if (periodo.Horas > 0 && periodo.Horas < horasPeriodo)
                    {
                        return "PARCIAL";
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            else
            {
                return "";
            }
        }


        public FiltroGenericoDtoBase<AprovarPagamentoDto> FiltrarAprovarPagamento(FiltroGenericoDtoBase<AprovarPagamentoDto> filtro)
        {

            filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();

            var mesAtual = DateTime.Now;
            var mesAntigo = DateTime.Now.AddMonths(-1);

            var query = DbSet.AsQueryable().AsNoTracking();
            query = query
                .Include(x => x.Pessoa)
                .Include(x => x.Cargo)
                .Include(x => x.Contratacao)
                .Include(x => x.DiaPagamento)
                .Include(x => x.TipoRemuneracao)
                .Include(x => x.ValoresPrestador)
                .Include(x => x.EmpresasPrestador)
                    .ThenInclude(x => x.Empresa)
                .Include(x => x.HorasMesPrestador)
                    .ThenInclude(h => h.HorasMes)
                .Include(x => x.HorasMesPrestador)
                    .ThenInclude(y => y.PrestadoresEnvioNf)
                .Include(x => x.HorasMesPrestador)
                    .ThenInclude(y => y.DescontosPrestador)
                .Include(x => x.Celula)
                    .ThenInclude(x => x.CelulaSuperior)
                        .ThenInclude(x => x.Pessoa)
                .Include(x => x.Celula)
                    .ThenInclude(x => x.Pessoa);

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelula));

            query = TratativasNoControlePagamento(filtro, query, out int idHorasMes, out string filtroTextoGenerico, mesAntigo, mesAtual);

            var horasMes = _horasMesRepository.BuscarPorId(idHorasMes);

            var dados = query.Select(p => new AprovarPagamentoDto
            {
                Id = p.Id,
                IdCelula = p.IdCelula,
                IdPessoaAprovador = ObterIdPessoaAprovador(p, idHorasMes),
                IdPessoaVisualizador = ObterIdPessoaVisualizador(p, idHorasMes),
                IdCelulaSuperior = p.Celula.IdCelulaSuperior,
                Cnpj = ObterCnpjDaEmpresaAtiva(p),
                Nome = p.Pessoa.Nome,
                EmailResponsavel = ObterEmailAprovador(p, idHorasMes),
                EmailVisualizador = ObterEmailVisualizador(p, idHorasMes),
                Contratacao = p.Contratacao.DescricaoValor,
                DiaPagamento = Convert.ToInt32(p.DiaPagamento.DescricaoValor),
                Horas = ObterHorasTotal(p, idHorasMes),
                Valor = ObterValor(p, idHorasMes, false, horasMes),
                ValorComDescontos = ObterValor(p, idHorasMes, true, horasMes),
                Tipo = ObterTipo(p, idHorasMes),
                Aprovador = ObterAprovador(p, idHorasMes),
                Status = p.DataDesligamento.HasValue ? "Inativo" : "Ativo",
                Desabilita = false,
                Situacao = ObterSituacao(p, idHorasMes),
                IdChaveOrigemIntRm = ObterChaveOrigemRm(p, idHorasMes),
                IdHoraMesPrestador = ObterIdHoraMesPrestador(p, idHorasMes),
                Aprovar = ObterAprovar(p, idHorasMes),
                NomeOperadorHoras = ObterNomeOperador(p, idHorasMes),
                PossuiNf = VerificarSePrestadorPossuiNf(p, idHorasMes)
            });

            var lista = dados.ToList();
            var listaVisualiza = dados.ToList();

            FiltrarPorSituacaoPossivelDeAprovacao(filtro, ref lista, ref listaVisualiza);
            lista = FiltrarPorResponsavelPorAprovacao(lista, listaVisualiza, filtro.Id == APROVAR_PAGAMENTOS);

            lista.ForEach(l =>
            {
                l.NomeEfetuaouUltimaALteracao = ObterNomeEfetuouUltimaAlteracao(l.IdHoraMesPrestador);
            });

            if (!String.IsNullOrEmpty(filtroTextoGenerico))
            {
                lista = lista.Where(x => x.IdCelula.ToString().Equals(filtroTextoGenerico.ToUpper())
                        || x.Nome.ToUpper().Contains(filtroTextoGenerico.ToUpper())
                        || (x.DiaPagamento.HasValue ? x.DiaPagamento.ToString().Equals(filtroTextoGenerico.ToUpper()) : false)
                        || (x.Horas.HasValue ? x.Horas.Value.Equals(filtroTextoGenerico.ToUpper()) : false)
                        || (x.Valor.ToString().Replace(".", "").Replace(",", "").Trim().Contains(filtroTextoGenerico.Replace(".", "").Replace(",", "").Replace("R$", "").Trim()))
                        || (x.Situacao != null ? x.Situacao.ToUpper().Equals(filtroTextoGenerico.ToUpper()) : false)
                        || (x.Aprovador != null ? x.Aprovador.ToUpper().Equals(filtroTextoGenerico.ToUpper()) : false)
                        || (x.Tipo != null ? x.Tipo.ToUpper().Equals(filtroTextoGenerico.ToUpper()) : false)
                        || (x.Cnpj != null ? x.Cnpj.Contains(filtroTextoGenerico.Replace(".", "").Replace("/", "").Replace("-", "").Trim()) : false))
                        .ToList();
            }

            if (filtro.Id != APROVAR_PAGAMENTOS && filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                List<int> valorParaFiltrarList = new List<int>();

                var valorParaFiltrarSplit = filtro.ValorParaFiltrar.Split(',');
                for (int i = 0; i < valorParaFiltrarSplit.Length; i++)
                {
                    valorParaFiltrarList.Add(int.Parse(valorParaFiltrarSplit[i]));
                }

                lista = lista.Where(x => valorParaFiltrarList.Any(y => y == x.IdCelula))
                        .ToList();
            }

            filtro.Total = lista.Count();

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                lista = lista.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                lista = lista.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else
            {
                lista = lista.OrderBy(y => y.IdCelula).ThenBy(x => x.Nome).ToList();
            }

            filtro.Valores = lista.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();
            return filtro;
        }

        private string ObterCnpjDaEmpresaAtiva(Prestador p) => p?.EmpresasPrestador?.FirstOrDefault(x => x.Empresa.Ativo)?.Empresa.Cnpj ?? null;

        private static IQueryable<Prestador> TratativasNoControlePagamento(FiltroGenericoDtoBase<AprovarPagamentoDto> filtro, IQueryable<Prestador> query, out int idHorasMes, out string filtroTextoGenerico, DateTime mesAntigo, DateTime mesAtual)
        {
            if (filtro.Id != APROVAR_PAGAMENTOS)
            {
                var filtroGenericoSplit = filtro.FiltroGenerico.Split(",");
                idHorasMes = Convert.ToInt32(filtroGenericoSplit[0]);

                query = query
                    .Include(x => x.Pessoa)
                    .Where(x => x.HorasMesPrestador.Any(y => y.IdHorasMes == Convert.ToInt32(filtroGenericoSplit[0])));

                int idDiaPagamento = Convert.ToInt32(filtroGenericoSplit[1]);
                filtroTextoGenerico = filtroGenericoSplit[2];
                filtroTextoGenerico = filtroTextoGenerico.Trim();

                if (idDiaPagamento != 0)
                {
                    query = query.Where(x => (x.IdDiaPagamento == idDiaPagamento));
                }
            }
            else
            {
                idHorasMes = Convert.ToInt32(filtro.FiltroGenerico);
                filtroTextoGenerico = filtro.ValorParaFiltrar;

                query = query.Where(x => !x.DataDesligamento.HasValue ||
                            (!x.DataDesligamento.HasValue || (x.DataDesligamento.Value.Month == mesAtual.Month && x.DataDesligamento.Value.Year == mesAtual.Year)
                                || (x.DataDesligamento.Value.Month == mesAntigo.Month && x.DataDesligamento.Value.Year == mesAntigo.Year)));
            }

            return query;
        }

        private List<AprovarPagamentoDto> FiltrarPorResponsavelPorAprovacao(List<AprovarPagamentoDto> lista, List<AprovarPagamentoDto> listaVisualiza, bool aprovarPagamentos)
        {
            if (aprovarPagamentos && !VerificarSeUsuarioVisualizaTudo())
            {
                var idPessoaLogada = _pessoaRepository.ObterIdPessoa(_variables.IdEacesso);
                lista = lista.Where(x =>
                        x.IdPessoaAprovador == idPessoaLogada)
                        .ToList();

                var somenteVisualiza = listaVisualiza.Where(x => !lista.Any(y => y.IdHoraMesPrestador == x.IdHoraMesPrestador)
                                       && (x.IdPessoaVisualizador == idPessoaLogada))
                                       .ToList();

                foreach (var item in somenteVisualiza)
                {
                    item.SomenteVisualiza = true;
                }

                lista = lista.Concat(somenteVisualiza).ToList();
            }

            return lista;
        }

        private static void FiltrarPorSituacaoPossivelDeAprovacao(FiltroGenericoDtoBase<AprovarPagamentoDto> filtro, ref List<AprovarPagamentoDto> lista, ref List<AprovarPagamentoDto> listaVisualiza)
        {
            if (filtro.Id == APROVAR_PAGAMENTOS)
            {
                lista = lista.Where(x =>
                            !x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription()) &&
                            !x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription())).ToList();

                listaVisualiza = listaVisualiza.Where(x =>
                            !x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription()) &&
                            !x.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_PENDENTE.GetDescription())).ToList();
            }
        }

        private bool VerificarSeUsuarioVisualizaTudo()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/UsuarioPerfil/verifica-prestador-master/" + _variables.UserName).Result;

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<bool?>(jsonString);
            return result ?? false;
        }

        private bool VerificarSePrestadorPossuiNf(Prestador p, int idHorasMes)
        {
            if (idHorasMes == 0)
            {
                return false;
            }
            else
            {
                var horasMesPrestador = p.HorasMesPrestador.Where(x => x.IdHorasMes == idHorasMes);
                if (horasMesPrestador.Any())
                {
                    var notasFiscais = horasMesPrestador.SelectMany(x => x.PrestadoresEnvioNf).Where(x => x.CaminhoNf != null);
                    if (notasFiscais.Any())
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        private string ObterNomeOperador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                return horasMes.Prestador.Pessoa.Nome;
            }
            return String.Empty;

        }

        private string ObterNomeEfetuouUltimaAlteracao(int? idHoraMesPrestador)
        {
            if (idHoraMesPrestador != null)
            {
                var horaMesPrestador = _horasMesPrestadorRepository.BuscarLancamentoParaPeriodoVigenteIdHoraMesPrestador((int)idHoraMesPrestador);
                return horaMesPrestador?.LogsHorasMesPrestador?.OrderByDescending(x => x.DataAlteracao)?.FirstOrDefault()?.Usuario ?? String.Empty;

            }
            return String.Empty;
        }

        private string ObterTipo(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horasMes.Extras == null && !LancadorIgualGerente(prestador))
                {
                    return "Gerente";
                }
                else
                {
                    return "Diretor";
                }
            }
            else
            {
                return "";
            }
        }

        public string ObterAprovador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horasMes.Extras == null && !LancadorIgualGerente(prestador))
                {
                    return prestador.Celula?.Pessoa?.Nome;
                }
                else
                {
                    return prestador.Celula?.CelulaSuperior?.Pessoa?.Nome;
                }
            }
            else
            {
                return null;
            }
        }

        private string ObterEmailAprovador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horasMes.Extras == null && !LancadorIgualGerente(prestador))
                {
                    return prestador.Celula?.Pessoa?.Email;
                }
                else
                {
                    return prestador.Celula.IdCelulaSuperior.HasValue ?
                                prestador.Celula?.CelulaSuperior.Pessoa?.Email : "";
                }
            }
            else
            {
                return "";
            }
        }

        private string ObterEmailVisualizador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horasMes.Extras == null && !LancadorIgualGerente(prestador))
                {
                    return prestador.Celula.IdCelulaSuperior.HasValue ?
                                prestador.Celula.CelulaSuperior.Pessoa?.Email : "";
                }
                else
                {
                    return prestador.Celula.Pessoa?.Email;
                }
            }
            else
            {
                return "";
            }
        }

        private int ObterIdPessoaAprovador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horasMes.Extras == null && !LancadorIgualGerente(prestador))
                {
                    return prestador.Celula.IdPessoaResponsavel ?? -1;
                }
                else
                {
                    return prestador.Celula.IdCelulaSuperior.HasValue ?
                                (prestador.Celula.CelulaSuperior.IdPessoaResponsavel ?? -1) : -1;
                }
            }
            else
            {
                return -1;
            }
        }

        private int ObterIdPessoaVisualizador(Prestador prestador, int idHorasMes)
        {
            var horasMes = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMes != null)
            {
                if (horasMes.Extras == null && !LancadorIgualGerente(prestador))
                {
                    return prestador.Celula.IdCelulaSuperior.HasValue ?
                                (prestador.Celula.CelulaSuperior.IdPessoaResponsavel ?? -1) : -1;
                }
                else
                {
                    return prestador.Celula.IdPessoaResponsavel ?? -1;
                }
            }
            else
            {
                return -1;
            }
        }

        private bool LancadorIgualGerente(Prestador prestador)
        {
            var idPessoaGerente = prestador.Celula?.IdPessoaResponsavel;
            var idPessoaPrestador = prestador.IdPessoa;

            return idPessoaGerente.HasValue && idPessoaPrestador == idPessoaGerente;
        }

        private decimal ObterValor(Prestador prestador, int idHorasMes, bool realizaDescontos, HorasMes horasMes)
        {
            var horasMesPrestador = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            if (horasMesPrestador != null)
            {
                var valorPrestadorAtual = ObterValorPrestadorAtual(prestador, horasMes);

                if (valorPrestadorAtual != null)
                {
                    if (horasMesPrestador.SemPrestacaoServico)
                    {
                        return 0;
                    }
                    else
                    {
                        decimal totalPagamentoMensal = prestador.TipoRemuneracao.DescricaoValor.Equals("MENSALISTA") ? ObterValorMensalista(valorPrestadorAtual, horasMesPrestador) :
                            (prestador.TipoRemuneracao.DescricaoValor.Equals("HORISTA") ?
                                (valorPrestadorAtual.ValorHora * (horasMesPrestador.Horas ?? 0)) : 0);

                        decimal totalHoraAdicional = 0;
                        if (horasMesPrestador.Extras.HasValue)
                        {
                            totalHoraAdicional = horasMesPrestador.Extras.Value * valorPrestadorAtual.ValorHora;
                        }

                        var valorTotal = totalPagamentoMensal + totalHoraAdicional;
                        if (realizaDescontos)
                        {
                            valorTotal = valorTotal - CalcularDescontos(horasMesPrestador);
                        }
                        return valorTotal;
                    }
                }
            }

            return 0;
        }

        private ValorPrestador ObterValorPrestadorAtual(Prestador prestador, HorasMes horasMes)
        {
            ValorPrestador valorAtual = null;

            var valorAtualList = prestador.ValoresPrestador.ToList().OrderByDescending(x => x.DataAlteracao);

            foreach (ValorPrestador valorPrestador in valorAtualList.ToList())
            {
                DateTime dataLimite;
                var mesVigencia = horasMes.Mes + 1;
                if (mesVigencia > 12)
                {
                    dataLimite = new DateTime(horasMes.Ano + 1, 1, 1);
                }
                else
                {
                    dataLimite = new DateTime(horasMes.Ano, mesVigencia, 1);
                }

                if (valorPrestador.DataAlteracao < dataLimite)
                {
                    valorAtual = valorPrestador;
                    break;
                }
            };
            return valorAtual;
        }

        private ValoresStfcorpDto ObterValoresStfcorp(Prestador prestador, int idHorasMes, bool realizaDescontos)
        {
            var horasMesPrestador = prestador.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == idHorasMes);
            var valoresStfcorpDto = new ValoresStfcorpDto();
            valoresStfcorpDto.ValorComDesconto = 0;
            valoresStfcorpDto.ValorTotal = 0;

            if (horasMesPrestador != null)
            {
                var valorAtual = ObterValorPrestadorAtual(prestador, horasMesPrestador.HorasMes);

                if (valorAtual != null)
                {
                    if (horasMesPrestador.SemPrestacaoServico)
                    {
                        return valoresStfcorpDto;
                    }
                    else
                    {
                        decimal totalPagamentoMensal = prestador.TipoRemuneracao.DescricaoValor.Equals("MENSALISTA") ? ObterValorMensalista(valorAtual, horasMesPrestador) :
                            (prestador.TipoRemuneracao.DescricaoValor.Equals("HORISTA") ?
                                (valorAtual.ValorHora * (horasMesPrestador.Horas ?? 0)) : 0);

                        decimal totalHoraAdicional = 0;
                        if (horasMesPrestador.Extras.HasValue)
                        {
                            totalHoraAdicional = horasMesPrestador.Extras.Value * valorAtual.ValorHora;
                        }


                        valoresStfcorpDto.ValorTotal = totalPagamentoMensal + totalHoraAdicional;
                        //if (realizaDescontos)
                        //{
                        valoresStfcorpDto.ValorComDesconto = valoresStfcorpDto.ValorTotal - CalcularDescontos(horasMesPrestador);
                        //}


                        return valoresStfcorpDto;
                    }
                }
            }

            return valoresStfcorpDto;
        }

        public decimal ObterValorMensalista(ValorPrestador valorPrestador, HorasMesPrestador horasMesPrestador)
        {
            if (valorPrestador != null)
            {
                var horasUteisMes = horasMesPrestador.HorasMes.Horas;
                if (horasMesPrestador.Horas.HasValue && (horasMesPrestador.Horas >= horasUteisMes))
                {
                    return valorPrestador.ValorMes;
                }
                else
                {
                    return valorPrestador.ValorHora * (horasMesPrestador.Horas ?? 0);
                }
            }
            return Decimal.Zero;
        }

        public decimal ObterValorHoraMensalista(ValorPrestador valorPrestador, HorasMesPrestador horasMesPrestador)
        {
            if (valorPrestador != null)
            {
                var horasUteisMes = horasMesPrestador.HorasMes.Horas;
                if (horasMesPrestador.Horas.HasValue && (horasMesPrestador.Horas >= horasUteisMes))
                {
                    return valorPrestador.ValorMes;
                }
                else
                {
                    return valorPrestador.ValorHora;
                }
            }
            return Decimal.Zero;
        }

        private decimal CalcularDescontos(HorasMesPrestador horasMesPrestador)
        {
            decimal descontos = 0;
            if (horasMesPrestador.DescontosPrestador != null)
            {
                descontos = horasMesPrestador.DescontosPrestador.Sum(x => x.ValorDesconto);
            }
            return descontos;
        }

        public DadosPagamentoPrestadorDto ObterDadosPagementoPorId(int id, int IdHorasMes)
        {
            var query = DbSet
                .Include(x => x.Pessoa)
                .Include(x => x.Cargo)
                .Include(x => x.Contratacao)
                .Include(x => x.DiaPagamento)
                .Include(x => x.TipoRemuneracao)
                .Include(x => x.ValoresPrestador)
                .Include(x => x.HorasMesPrestador)
                    .ThenInclude(x => x.LogsHorasMesPrestador)
                .Include(x => x.HorasMesPrestador)
                    .ThenInclude(x => x.HorasMes)
                .Include(x => x.HorasMesPrestador)
                    .ThenInclude(x => x.DescontosPrestador)
                        .ThenInclude(x => x.Desconto)
                .FirstOrDefault(x => x.Id == id);

            List<DetalhePagamentoPretadorDto> detalhePagamentoPretadorDtoList = new List<DetalhePagamentoPretadorDto>();
            List<HistoricoPagamentoPrestadorDto> historicoPagamentoPrestadorDtoList = new List<HistoricoPagamentoPrestadorDto>();

            var valoresPrestador = ObterValorPrestadorAtual(query, query.HorasMesPrestador.FirstOrDefault(x => x.IdHorasMes == IdHorasMes).HorasMes);

            var valorMes = valoresPrestador != null ? valoresPrestador.ValorMes : 0;
            var valorHora = valoresPrestador != null ? valoresPrestador.ValorHora : 0;

            var tipoRemuneracao = query.TipoRemuneracao != null ? query.TipoRemuneracao.DescricaoValor : "HORISTA";
            query.HorasMesPrestador.ToList().ForEach(h =>
            {
                if (h.IdHorasMes == IdHorasMes)
                {
                    h.LogsHorasMesPrestador.ToList().ForEach(l =>
                    {
                        var historicoDto = new HistoricoPagamentoPrestadorDto
                        {
                            Id = l.Id,
                            Data = l.DataAlteracao,
                            Login = l.Usuario,
                            Motivo = l.DescMotivo,
                            Status = l.SituacaoNova
                        };
                        historicoPagamentoPrestadorDtoList.Add(historicoDto);
                    });

                    var detalhePagamentoPretadorDto = new DetalhePagamentoPretadorDto
                    {
                        Tipo = "Pagamento Mensal",
                        Quantidade = h.SemPrestacaoServico ? 0 : tipoRemuneracao.Equals("MENSALISTA") ? ObterQuantidadeMensalista(h) : (h.Horas ?? 0),
                        ValorUnitario = tipoRemuneracao.Equals("MENSALISTA") ? ObterValorHoraMensalista(valoresPrestador, h) : valorHora,
                        Total = h.SemPrestacaoServico ? 0 : tipoRemuneracao.Equals("MENSALISTA") ? ObterValorMensalista(valoresPrestador, h) : valorHora * (h.Horas ?? 0),
                        Observacao = h.Extras.HasValue ? String.Empty : h.ObservacaoSemPrestacaoServico
                    };
                    detalhePagamentoPretadorDtoList.Add(detalhePagamentoPretadorDto);

                    if (h.Extras.HasValue)
                    {
                        var detalhePagamentoPretadorDtoExtra = new DetalhePagamentoPretadorDto
                        {

                            Tipo = "Hora Adicional",
                            Quantidade = h.Extras,
                            ValorUnitario = valorHora,
                            Total = h.Extras.Value * valorHora,
                            Observacao = h.ObservacaoSemPrestacaoServico
                        };
                        detalhePagamentoPretadorDtoList.Add(detalhePagamentoPretadorDtoExtra);
                    }

                    if (h.DescontosPrestador.Any())
                    {
                        var detalhePagamentoPretadorDtoExtra = new DetalhePagamentoPretadorDto
                        {

                            Tipo = "Descontos",
                            Quantidade = h.DescontosPrestador.Count,
                            ValorUnitario = 0,
                            Total = h.DescontosPrestador.Sum(x => x.ValorDesconto) * (-1),
                            Observacao = string.Join(", ", h.DescontosPrestador.Select(x => x.Desconto.DescricaoValor))
                        };
                        detalhePagamentoPretadorDtoList.Add(detalhePagamentoPretadorDtoExtra);
                    }

                    if (detalhePagamentoPretadorDtoList.Count > 1)
                    {
                        var detalhePagamentoPretadorDtoTotal = new DetalhePagamentoPretadorDto
                        {

                            Tipo = "Total",
                            Quantidade = null,
                            ValorUnitario = 0,
                            Total = detalhePagamentoPretadorDtoList.Sum(x => x.Total),
                            Observacao = String.Empty
                        };
                        detalhePagamentoPretadorDtoList.Add(detalhePagamentoPretadorDtoTotal);
                    }

                }
            });

            var historicoPagamentoPrestadorDtoListOrder = historicoPagamentoPrestadorDtoList.OrderBy(h => h.Id).ToList();

            var dados = new DadosPagamentoPrestadorDto
            {
                dataDesligamento = query.DataDesligamento,
                dataInicio = query.DataInicio,
                detalhesPagamentoPrestador = detalhePagamentoPretadorDtoList,
                historicosPagamentoPrestador = historicoPagamentoPrestadorDtoListOrder
            };

            return dados;
        }

        public decimal ObterQuantidadeMensalista(HorasMesPrestador horasMesPrestador)
        {
            decimal quantidade = 0;
            if (horasMesPrestador.Horas.HasValue && horasMesPrestador.Horas.Value >= horasMesPrestador.HorasMes.Horas)
            {
                quantidade = 1;
            }
            else if (horasMesPrestador.Horas.HasValue && horasMesPrestador.Horas.Value < horasMesPrestador.HorasMes.Horas)
            {
                quantidade = horasMesPrestador.Horas.Value;
            }

            return quantidade;
        }

        public List<Prestador> BuscarTodosSemTracking()
        {
            var result = DbSet.AsNoTracking().ToList();
            return result;
        }

        public FiltroGenericoDtoBase<ConciliacaoPagamentoDto> FiltrarConciliacaoPagamentos(FiltroGenericoDtoBase<ConciliacaoPagamentoDto> filtro, bool resumo)
        {
            if (!string.IsNullOrEmpty(filtro.ValorParaFiltrar))
            {
                filtro.ValorParaFiltrar = filtro.ValorParaFiltrar.Trim();
            }
            
            var mesAtual = DateTime.Now;
            var mesAntigo = DateTime.Now.AddMonths(-1);

            var filtroGenericoSplit = filtro.FiltroGenerico.Split(",");
            int idDiaPagamento = Convert.ToInt32(filtroGenericoSplit[1]);

            var query = DbSet.AsQueryable();
            query = query
                .Include(x => x.Pessoa)
                .Include(x => x.Contratacao)
                .Include(x => x.DiaPagamento)
                .Include(x => x.TipoRemuneracao)
                .Include(x => x.ValoresPrestador)
                .Include(x => x.ClientesServicosPrestador)
                .Include(x => x.HorasMesPrestador).ThenInclude(x => x.HorasMes)
                .Include(x => x.Celula)
                                .ThenInclude(x => x.CelulaSuperior)
                .Include(x => x.EmpresasPrestador)
                            .ThenInclude(x => x.Empresa)
                .Where(x => x.IdTipoRemuneracao.HasValue &&
                            (!x.DataDesligamento.HasValue || (x.DataDesligamento.Value.Month == mesAtual.Month && x.DataDesligamento.Value.Year == mesAtual.Year)
                                || (x.DataDesligamento.Value.Month == mesAntigo.Month && x.DataDesligamento.Value.Year == mesAntigo.Year)));

            int idHorasMes = Convert.ToInt32(filtroGenericoSplit[0]);

            if (idDiaPagamento != 0)
            {
                query = query.Where(x => (x.IdDiaPagamento == idDiaPagamento));
            }

            query = query.Where(x => x.HorasMesPrestador.Any(y => y.IdHorasMes == idHorasMes
                                    && y.Situacao != SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription()));

            query = query.Where(x => _variables.CelulasComPermissao.Any(y => y == x.IdCelula));

            var diaPagamentos = _dominioRepository.Buscar(x => x.ValorTipoDominio.Equals("DIA_PAGAMENTO")).ToList();
            var dados = query.ToList();

            var lista = new List<ConciliacaoPagamentoDto>();

            dados.ForEach(p =>
            {

                var valoresStfcorp = ObterValoresStfcorp(p, idHorasMes, false);
                var conciliacaoPagamentoDto = new ConciliacaoPagamentoDto
                {
                    IdCelula = p.IdCelula,
                    Nome = p.Pessoa.Nome,
                    ValorStfcorp = valoresStfcorp.ValorTotal,
                    ValorStfcorpComDescontos = valoresStfcorp.ValorComDesconto,
                    EmpresasPrestador = p.EmpresasPrestador.ToList(),
                    IdEmpresaGrupo = p.IdEmpresaGrupo,
                    IdChaveOrigemIntRm = p.HorasMesPrestador.Where(x => x.IdHorasMes == idHorasMes).FirstOrDefault().IdChaveOrigemIntRm,
                    Usuario = p.Usuario,
                    DataAlteracao = p.DataAlteracao,
                    Empresa = p.EmpresasPrestador.Select(x => x.Empresa).Any(x => x.Ativo) ? p.EmpresasPrestador.Select(x => x.Empresa).FirstOrDefault(x => x.Ativo).RazaoSocial : "",
                    Diretoria = p.Celula.CelulaSuperior.Descricao,
                    Contratacao = p.Contratacao.DescricaoValor,
                    ClientesServicosPrestador = p.ClientesServicosPrestador,
                    DiaPagamento = Convert.ToInt32(diaPagamentos.FirstOrDefault(x => x.Id == (int)p.DiaPagamento.Id).DescricaoValor)
                };
                adiconarDadosResumo(conciliacaoPagamentoDto);
                lista.Add(conciliacaoPagamentoDto);
            });

            if (filtro.ValorParaFiltrar != null && filtro.ValorParaFiltrar.Any())
            {
                lista = lista.Where(x => x.IdCelula.ToString().Equals(filtro.ValorParaFiltrar.ToUpper())
                                    || x.Nome.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())
                                    || x.Empresa.ToUpper().Contains(filtro.ValorParaFiltrar.ToUpper())).ToList();
            }

            filtro.CampoOrdenacao = filtro.CampoOrdenacao.First().ToString().ToUpper() + filtro.CampoOrdenacao.Substring(1);
            if (filtro.OrdemOrdenacao == "asc")
            {
                lista = lista.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                lista = lista.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else
            {
                lista = lista.OrderBy(y => y.IdCelula).ThenBy(y => y.Nome).ToList();
            }

            filtro.Valores = lista.ToList();

            return filtro;

        }

        public IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula)
        {
            var result = DbSet.Include(x => x.Pessoa).Where(x => x.IdCelula == idCelula && !x.DataDesligamento.HasValue).Select(x => new KeyValueDto
            {
                Id = x.Id,
                Nome = x.Pessoa.Nome
            }).ToList();

            return result;
        }

        public Prestador ObterPrestadorParaTransferencia(int idPrestador)
        {
            var prestador = DbSet.AsNoTracking()
                .Include(x => x.Pessoa)
                .Include(x => x.Celula)
                .Include(x => x.ClientesServicosPrestador)
                .FirstOrDefault(x => x.Id == idPrestador);

            if (prestador == null) return null;

            prestador.ClientesServicosPrestador = prestador.ClientesServicosPrestador.Where(x => x.Ativo).ToList();

            return prestador;
        }

        public IEnumerable<Prestador> ObterPrestadoresParaTransferencia(int[] idsPrestadores)
        {
            var prestadores = DbSet.AsNoTracking()
                .Include(x => x.Celula)
                .Include(x => x.ClientesServicosPrestador)
                .Where(x => idsPrestadores.Contains(x.Id))
                .ToList();


            prestadores = prestadores.Select(x =>
            {
                x.ClientesServicosPrestador = x.ClientesServicosPrestador.Where(y => y.Ativo).ToList();
                return x;
            }).ToList();


            return prestadores;
        }

        public ClienteET ObterClientePrestador(int idCliente)
        {
            var cliente = _context.Set<ClienteET>().AsNoTracking().FirstOrDefault(x => x.Id == idCliente);

            return cliente;
        }

        public ComboDefaultDto ObterClienteAtivoPorIdCelulaEAcesso(int idCelula, int idCliente)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = $@"select DISTINCT c.idCliente as id, c.nomeFantasia as descricao from 
                                stfcorp.tblclientes c, stfcorp.tblClientesServicos s 
                                where c.idCliente = s.idCliente and DtInativacao is Null and idCelula= {idCelula} and c.idCliente = {idCliente}
                                ORDER BY descricao;";
                var cliente = dbConnection.QueryFirstOrDefault<ComboDefaultDto>(query);
                dbConnection.Close();

                return cliente;
            }
        }

        public ComboLocalDto ObterLocalTrabalhoPorId(int idLocal, int idCliente)
        {
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select DISTINCT l.IdLocal as id, l.Nome as descricao, lo.Logradouro as tipo,l.Endereco as endereco, l.Numero as numero, " +
                            "l.CompEndereco as complemento, l.Bairro as bairro, l.cep as cep, cid.Cidade as cidade, e.Estado estado, p.Pais pais " +
                            "from stfcorp.TblClientesLocaisTrabalho l, stfcorp.tblCidades cid, stfcorp.tblClientes c, stfcorp.tblEstados e, stfcorp.tblpaises p, stfcorp.tbllogradouros lo " +
                            "where l.Inativo = 0 and cid.IdCidade = l.IdCidade and cid.SiglaEstado = e.SiglaEstado and cid.SiglaPais = p.SiglaPais and l.abrevlogradouro = lo.abrevlogradouro and " +
                            "l.IdLocal = " + idLocal + " and l.IdCliente = " + idCliente;
                var local = dbConnection.QueryFirstOrDefault<ComboLocalDto>(query);
                dbConnection.Close();
                return local;
            }
        }

        public Prestador ObterPorIdComInativacoes(int idPrestador)
        {
            var prestador = DbSet.Include(x => x.InativacoesPrestador).FirstOrDefault(x => x.Id == idPrestador);

            return prestador;
        }

        private void adiconarDadosResumo(ConciliacaoPagamentoDto item)
        {
            var valoresRM = _rmService.ObterValoresRm(item.IdEmpresaGrupo, item.IdChaveOrigemIntRm);
            if (valoresRM != null)
            {
                item.ValorRm = valoresRM.ValorBruto;
                item.ValorRmComDesconto = valoresRM.ValorLiquido;
                item.StatusRm = valoresRM.Status ?? "A";
            }

            item.Conciliado = (item.ValorStfcorpComDescontos == item.ValorRm) && (item.ValorStfcorpComDescontos != 0) && (item.ValorRm != 0);

            int diaAtual = DateTime.Now.Day;
            if (item.StatusRm != "Q")
            {
                definirStatusPagamento(item.DiaPagamento, item, diaAtual);
            }
            item.Fechado = (item.StatusRm == "Q") ? true : false;

            var clienteServicoAtivo = item.ClientesServicosPrestador.FirstOrDefault(x => x.Ativo);
            string codCCusto = String.Empty;
            if (clienteServicoAtivo != null)
            {
                item.CodigoCentroCusto = ("000" + clienteServicoAtivo.IdCelula).Substring((("000" + clienteServicoAtivo.IdCelula).Length) - 4) + "." +
                            ("0000" + clienteServicoAtivo.IdCliente).Substring((("0000" + clienteServicoAtivo.IdCliente).Length) - 5) + "." +
                                ("00000" + clienteServicoAtivo.IdServico).Substring((("00000" + clienteServicoAtivo.IdServico).Length) - 6);
            }
            item.ClientesServicosPrestador = null;

            item.CentroCusto = (item.CodigoCentroCusto != null ? true : false);
        }

        private void definirStatusPagamento(int valorDiaPagamento, ConciliacaoPagamentoDto item, int diaAtual)
        {
            if (valorDiaPagamento > 1 && valorDiaPagamento >= diaAtual)
            {
                item.StatusRm = "DP";
            }
            else
            {
                item.StatusRm = "FP";
            }
        }

        private Empresa obterDadosEmpresa(ICollection<EmpresaPrestador> empresasPrestador)
        {
            var empresaAtiva = empresasPrestador.Select(x => x.Empresa).FirstOrDefault(x => x.Ativo);
            if (empresaAtiva != null)
            {
                return empresaAtiva;
            }
            else
            {
                var ultimaEmpresa = empresasPrestador.Select(x => x.Empresa).OrderByDescending(x => x.Id).FirstOrDefault();
                if (ultimaEmpresa != null)
                {
                    return ultimaEmpresa;
                }
                else
                {
                    return null;
                }
            }
        }


    }
   
}

