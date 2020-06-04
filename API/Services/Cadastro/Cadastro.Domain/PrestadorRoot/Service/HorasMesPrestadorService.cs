using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.EmailRoot.DTO;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Repository;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Repository;
using Cadastro.Domain.PessoaRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.Extensions;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class HorasMesPrestadorService : IHorasMesPrestadorService
    {
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IHorasMesPrestadorRepository _horasMesPrestadorRepository;
        private readonly IHorasMesRepository _horasMesRepository;
        private readonly ILogHorasMesPrestadorRepository _logHorasMesPrestadorRepository;
        private readonly ICelulaRepository _celulaRepository;
        private readonly IPrestadorRepository _prestadorRepository;
        private readonly IPrestadorEnvioNfRepository _prestadorEnvioNfRepository;
        private readonly IPrestadorService _prestadorService;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEmpresaService _empresaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IVariablesToken _variables;

        public HorasMesPrestadorService(
            MicroServicosUrls microServicosUrls,
            IHorasMesPrestadorRepository horasMesPrestadorRepository,
            ICelulaRepository celulaRepository,
            ILogHorasMesPrestadorRepository logHorasMesPrestadorRepository,
            IHorasMesRepository horasMesRepository,
            IPrestadorRepository prestadorRepository,
            IPrestadorEnvioNfRepository prestadorEnvioNfRepository,
            IEmpresaService empresaService,
            IOptions<ConnectionStrings> connectionStrings,
            IPessoaRepository pessoaRepository,
            IPrestadorService prestadorService,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _horasMesPrestadorRepository = horasMesPrestadorRepository;
            _prestadorRepository = prestadorRepository;
            _celulaRepository = celulaRepository;
            _horasMesRepository = horasMesRepository;
            _logHorasMesPrestadorRepository = logHorasMesPrestadorRepository;
            _microServicosUrls = microServicosUrls;
            _prestadorEnvioNfRepository = prestadorEnvioNfRepository;
            _prestadorService = prestadorService;
            _connectionStrings = connectionStrings;
            _empresaService = empresaService;
            _pessoaRepository = pessoaRepository;
            _unitOfWork = unitOfWork;
            _variables = variables;
        }

        public void SalvarHoras(HorasMesPrestador prestadorHoras)
        {
            var horaAtual = _horasMesPrestadorRepository.BuscarLancamentoParaPeriodoVigente(prestadorHoras.IdPrestador, prestadorHoras.IdHorasMes);
            var horaAprovada = !prestadorHoras.Situacao.Equals(SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription())
                               && VerificaSeLancadorEhAprovador(prestadorHoras);
            if (prestadorHoras.SemPrestacaoServico)
            {
                prestadorHoras.Extras = null;
                prestadorHoras.Horas = null;
            }
            if (horaAtual != null)
            {
                horaAtual.Situacao = prestadorHoras.Situacao;
                horaAtual.SemPrestacaoServico = prestadorHoras.SemPrestacaoServico;
                AdicionarLogHorasCadastradas(horaAtual, horaAprovada, true, _variables.UserName);

                horaAtual.Horas = prestadorHoras.Horas;
                horaAtual.Extras = prestadorHoras.Extras;
                horaAtual.Situacao = ObterUltimaSituacao(horaAtual);
                horaAtual.ObservacaoSemPrestacaoServico = prestadorHoras.ObservacaoSemPrestacaoServico;
                horaAtual.SemPrestacaoServico = prestadorHoras.SemPrestacaoServico;
                _horasMesPrestadorRepository.Update(horaAtual);
            }
            else
            {
                AdicionarLogHorasCadastradas(prestadorHoras, horaAprovada, false, _variables.UserName);
                prestadorHoras.Situacao = ObterUltimaSituacao(prestadorHoras);
                _horasMesPrestadorRepository.Adicionar(prestadorHoras);
            }

            _unitOfWork.Commit();
            if (horaAprovada)
            {
                CriarRegistroPrestadorNf(prestadorHoras.Id);
                _unitOfWork.Commit();
            }
        }

        private string ObterUltimaSituacao(HorasMesPrestador prestadorHoras)
        {
            var situacao = prestadorHoras.LogsHorasMesPrestador.OrderByDescending(x => x.DataAlteracao).ThenByDescending(x => x.Id).FirstOrDefault().SituacaoNova;
            return situacao;
        }

        private static void AdicionarLogHorasCadastradas(HorasMesPrestador prestadorHoras, bool horaAprovada, bool recadastro, string usuario)
        {
            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = prestadorHoras.LogsHorasMesPrestador.LastOrDefault()?.SituacaoNova,
                SituacaoNova = DefinirSituacao(prestadorHoras, recadastro),
                DataAlteracao = DateTime.Now,
                Usuario = usuario
            };
            prestadorHoras.LogsHorasMesPrestador.Add(log);

            if (horaAprovada)
            {
                LogHorasMesPrestador logAprovado = new LogHorasMesPrestador
                {
                    SituacaoAnterior = DefinirSituacao(prestadorHoras, recadastro),
                    SituacaoNova = SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_APROVADAS.GetDescription(),
                    DataAlteracao = DateTime.Now,
                    Usuario = usuario

                };
                prestadorHoras.LogsHorasMesPrestador.Add(logAprovado);
            }
        }

        private static string DefinirSituacao(HorasMesPrestador prestadorHoras, bool recadastro)
        {
            if (prestadorHoras.SemPrestacaoServico)
            {
                return SharedEnuns.TipoSituacaoHorasMesPrestador.CANCELADO.GetDescription();
            }
            else
            {
                return recadastro ?
                    SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_RECADASTRADAS.GetDescription() :
                    SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_CADASTRADAS.GetDescription();
            }
        }

        private bool VerificaSeLancadorEhAprovador(HorasMesPrestador prestadorHoras)
        {
            var prestador = _prestadorRepository.BuscarPorIdComIncludeCelula(prestadorHoras.IdPrestador);

            var idPessoaLancador = _pessoaRepository.ObterIdPessoa(_variables.IdEacesso);
            var idPessoaPrestador = prestador.IdPessoa;
            var idPessoaAprovadorGerente = prestador.Celula?.IdPessoaResponsavel;
            var idPessoaAprovadorDiretor = prestador.Celula?.CelulaSuperior?.IdPessoaResponsavel;

            if (prestadorHoras.Extras.HasValue && prestadorHoras.Extras != 0)
            {
                return (idPessoaLancador == idPessoaAprovadorDiretor && idPessoaPrestador != idPessoaAprovadorDiretor);
            }
            else
            {
                var lancadorGerente = (idPessoaLancador == idPessoaAprovadorGerente && idPessoaPrestador != idPessoaAprovadorGerente);
                var lancadorDiretor = (idPessoaLancador == idPessoaAprovadorDiretor && idPessoaPrestador != idPessoaAprovadorDiretor);

                return lancadorGerente || lancadorDiretor;
            }
        }

        public void DefinirSituacaoNfHorasMesPrestador(PrestadorEnvioNf prestadorEnvioNF)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorId(prestadorEnvioNF.IdHorasMesPrestador);

            var listaDeRegistroDeNfs = _prestadorEnvioNfRepository.BuscarPorIdHorasMesPrestador(prestadorEnvioNF.IdHorasMesPrestador)?.ToList();
            bool arquivoDeNfJaFoiEnviado = !listaDeRegistroDeNfs.Any(x => x.CaminhoNf == null);
            if (arquivoDeNfJaFoiEnviado)
            {
                AdicionaNovoRegistroDeNF(prestadorEnvioNF);
                AdicionarLogHorasMesPrestador(prestadorEnvioNF.IdHorasMesPrestador,
                horasMesPrestador.Situacao,
                SharedEnuns.TipoSituacaoHorasMesPrestador.AGUARDANDO_ENTRADA_DA_NF.GetDescription()
                );
            }
            else
            {
                prestadorEnvioNF.CaminhoNf = prestadorEnvioNF.Token;
                _prestadorEnvioNfRepository.Update(prestadorEnvioNF);
            }

            _horasMesPrestadorRepository.Update(horasMesPrestador);
        }

        public int BuscarPorIdHoraMes(int id)
        {
            var periodo = _horasMesPrestadorRepository.BuscarPorIdHoraMes(id);
            return periodo;
        }

        public void AprovarPagamento(int idHoraMesPrestador)
        {
            var horaMesPrestador = _horasMesPrestadorRepository.BuscarLancamentoParaPeriodoVigenteIdHoraMesPrestador(idHoraMesPrestador);
            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = horaMesPrestador.Situacao,
                SituacaoNova = SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_APROVADAS.GetDescription(),
                DataAlteracao = DateTime.Now,
                Usuario = _variables.UserName
            };
            horaMesPrestador.LogsHorasMesPrestador.Add(log);
            horaMesPrestador.Situacao = SharedEnuns.TipoSituacaoHorasMesPrestador.HORAS_APROVADAS.GetDescription();
            _horasMesPrestadorRepository.Update(horaMesPrestador);

            CriarRegistroPrestadorNf(idHoraMesPrestador);

            if (_unitOfWork.Commit())
            {
                _prestadorService.SolicitarNF(idHoraMesPrestador);
            }
        }

        public void NegarPagamento(int idHoraMesPrestador, string motivo)
        {
            var horaMesPrestador = _horasMesPrestadorRepository.BuscarLancamentoParaPeriodoVigenteIdHoraMesPrestador(idHoraMesPrestador);

            LogHorasMesPrestador log = new LogHorasMesPrestador
            {
                SituacaoAnterior = horaMesPrestador.LogsHorasMesPrestador.OrderByDescending(x => x.DataAlteracao).FirstOrDefault().SituacaoNova,
                SituacaoNova = SharedEnuns.TipoSituacaoHorasMesPrestador.NEGADO.GetDescription(),
                DataAlteracao = DateTime.Now,
                Usuario = _variables.UserName,
                DescMotivo = motivo
            };

            horaMesPrestador.LogsHorasMesPrestador.Add(log);
            horaMesPrestador.Situacao = SharedEnuns.TipoSituacaoHorasMesPrestador.NEGADO.GetDescription();
            _horasMesPrestadorRepository.Update(horaMesPrestador);
            _unitOfWork.Commit();
        }

        public void EnviarEmailParaAprovacaoHoras()
        {
            var periodoVigente = _horasMesRepository.BuscarPeriodoVigente();
            var aprovacoesPendentes = _horasMesPrestadorRepository.BuscarAprovacoesPendentes(periodoVigente.Id);
            List<List<HorasMesPrestador>> aprovacoesPendentesPorDiaPagamento = new List<List<HorasMesPrestador>>();

            if (aprovacoesPendentes.Any())
            {
                var diasPagamento = periodoVigente.PeriodosDiaPagamento;

                foreach (var dia in diasPagamento)
                {
                    var aprovacoesDia = aprovacoesPendentes.Where(x => x.Prestador.IdDiaPagamento == dia.IdDiaPagamento).ToList();
                    if (aprovacoesDia.Any())
                    {
                        aprovacoesPendentesPorDiaPagamento.Add(aprovacoesDia);
                    }
                }

                foreach (var prestadores in aprovacoesPendentesPorDiaPagamento)
                {
                    var limitesPeriodoVigente = periodoVigente.PeriodosDiaPagamento.FirstOrDefault(x =>
                                                        x.IdDiaPagamento == prestadores.FirstOrDefault().Prestador.IdDiaPagamento);
                    var diaLimite = limitesPeriodoVigente.DiaLimiteAprovacaoHoras;

                    var horasFaltando = (diaLimite.Date - DateTime.Now.Date).TotalHours;

                    if (horasFaltando == 0)
                    {
                        var celulasGerenteAprova = prestadores.Where(x => !x.Extras.HasValue).Select(x => x.Prestador.Celula).ToList();
                        var celulasDiretorAprova = prestadores.Where(x => x.Extras.HasValue).Select(x => x.Prestador.Celula.CelulaSuperior).ToList();
                        var celulasParaEnvio = celulasGerenteAprova.Concat(celulasDiretorAprova).GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList();
                        var celulasParaEnvioComDistinctResponsavel = celulasParaEnvio.GroupBy(x => x.Pessoa.Email).Select(x => x.FirstOrDefault()).ToList();

                        List<BroadCastAprovacaoHorasDto> broadcasts = new List<BroadCastAprovacaoHorasDto>();
                        foreach (var celula in celulasParaEnvioComDistinctResponsavel)
                        {
                            List<ValorParametroEmailDTO> parametros = DefinirParametros(periodoVigente, celula, diaLimite, limitesPeriodoVigente.DiaPagamento.DescricaoValor);


                            var email = new EmailDTO
                            {
                                IdTemplate = 2,
                                RemetenteNome = "Gestão de Contratos",
                                Para = ObterDestinatarioEmailParaAprovacaoHoras(celula),
                                ValoresParametro = parametros
                            };
                            //EnvioEmail
                            EnviarEmail(email);
                            broadcasts.Add(MontarEntidadeBroadcastAprovacaoHoras(periodoVigente, celula, diaLimite, limitesPeriodoVigente.DiaPagamento.DescricaoValor));
                        }
                        CadastrarBroadcastsParaAberturaAprovacaoDeHoras(broadcasts);
                    }
                }
            }
        }

        private BroadCastAprovacaoHorasDto MontarEntidadeBroadcastAprovacaoHoras(HorasMes periodoVigente, Celula celula, DateTime diaLimite, string diaPagamento)
        {
            return new BroadCastAprovacaoHorasDto
            {
                NomeAprovador = celula.Pessoa.Nome,
                LoginAprovador = celula.Pessoa.Email.Split('@')[0],
                PeriodoCompetencia = new DateTime(periodoVigente.Ano, periodoVigente.Mes, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("pt")).ToUpper() + "/" + periodoVigente.Ano,
                DiaLimite = diaLimite.ToString("dd/MM/yyyy 23:59"),
                DiaPagamento = diaPagamento,
                Link = _microServicosUrls.UrlBase + "#/stfcorp/gestao-de-terceiros/aprovar-pagamentos-prestadores/consultar"
            };
        }

        private bool CadastrarBroadcastsParaAberturaAprovacaoDeHoras(List<BroadCastAprovacaoHorasDto> broadcasts)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);
                var requestBody = new StringContent(JsonConvert.SerializeObject(broadcasts), Encoding.UTF8, "application/json");
                var res = http.PutAsync("api/broadcast/criar-broadcasts-aprovacao-horas", requestBody).Result;
                var json = res.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<StfCorpHttpListResult<BroadcastDto>>(json);
                return result.Success;
            }
        }

        private string ObterDestinatarioEmailParaAprovacaoHoras(Celula celula)
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = "clbatista@stefanini.com";
                    break;
                case "STAGING":
                    destinatario = "clbatista@stefanini.com";
                    break;
                case "PRODUCTION":
                    destinatario = celula.Pessoa.Email;
                    break;
            }
            return destinatario;
        }

        private void CriarRegistroPrestadorNf(int idHorasMesPrestador)
        {
            var prestadorEnvioNf = new PrestadorEnvioNf
            {
                IdHorasMesPrestador = idHorasMesPrestador,
                Token = Guid.NewGuid().ToString(),
            };

            _prestadorEnvioNfRepository.Adicionar(prestadorEnvioNf);
        }

        private void EnviarEmail(EmailDTO email)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlEnvioEmail);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/Email", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private List<ValorParametroEmailDTO> DefinirParametros(HorasMes periodoVigente, Celula celula, DateTime diaLimite, string diaPagamento)
        {
            var parametros = new List<ValorParametroEmailDTO>();
            var parametroNome = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroNomeAprovador]",
                ParametroValor = celula.Pessoa.Nome
            };
            parametros.Add(parametroNome);

            var parametroPeriodo = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroPeriodoCompetencia]",
                ParametroValor = new DateTime(periodoVigente.Ano, periodoVigente.Mes, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("pt")).ToUpper() + "/" + periodoVigente.Ano
            };
            parametros.Add(parametroPeriodo);

            var parametroDiaLimite = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDiaLimite]",
                ParametroValor = diaLimite.ToString("dd/MM/yyyy 23:59")
            };
            parametros.Add(parametroDiaLimite);

            var parametroDiaPagamento = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDiaPagamento]",
                ParametroValor = diaPagamento
            };
            parametros.Add(parametroDiaPagamento);

            var parametroLink = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroLink]",
                ParametroValor = _microServicosUrls.UrlBase + "#/stfcorp/gestao-de-terceiros/aprovar-pagamentos-prestadores/consultar"
            };
            parametros.Add(parametroLink);
            return parametros;
        }


        public HorasMesPrestador BuscarPorId(int id)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorId(id);
            return horasMesPrestador;
        }

        public HorasMesPrestador BuscarPorIdComInclude(int id)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.BuscarPorIdComIncludes(id);
            return horasMesPrestador;
        }

        public List<HorasMesPrestador> BuscarLancamentosAprovados()
        {
            var periodoVigente = _horasMesRepository.BuscarPeriodoVigente();
            var lancamentosMes = _horasMesPrestadorRepository.BuscarLancamentosAprovados(periodoVigente.Id);
            return lancamentosMes;
        }

        public List<HorasMesPrestador> BuscarLancamentosComPagamentoPendente(int idHorasMes)
        {
            var lancamentosMes = _horasMesPrestadorRepository.BuscarLancamentosComPagamentoPendente(idHorasMes);
            return lancamentosMes;
        }

        public List<HorasMesPrestador> BuscarLancamentosComPagamentoSolicitado()
        {
            var periodoVigente = _horasMesRepository.BuscarPeriodoVigente();
            var lancamentosMes = _horasMesPrestadorRepository.BuscarLancamentosComPagamentoSolicitado(periodoVigente.Id);
            return lancamentosMes;
        }

        public List<HorasMesPrestador> BuscarHorasMesPrestadorPorPrestador(int idPrestador)
        {
            var horasMesPrestador = _horasMesPrestadorRepository.Buscar(x => x.IdPrestador == idPrestador).OrderByDescending(x => x.DataAlteracao).ToList();
            return horasMesPrestador;
        }

        private void AdicionaNovoRegistroDeNF(PrestadorEnvioNf prestadorEnvioNf)
        {
            var clone = prestadorEnvioNf.Clone();
            clone.Id = 0;
            _prestadorEnvioNfRepository.Adicionar(clone);
        }

        private void AdicionarLogHorasMesPrestador(int idHorasMesPrestador, string descricaoSituacaoAnterior, string descricaoSituacaoNova)
        {
            var registroLog = new LogHorasMesPrestador
            {
                SituacaoAnterior = descricaoSituacaoAnterior,
                SituacaoNova = descricaoSituacaoNova,
                DataAlteracao = DateTime.Now,
                Usuario = _variables.UserName,
                IdHorasMesPrestador = idHorasMesPrestador
            };
            _logHorasMesPrestadorRepository.Adicionar(registroLog);
        }

        public void Commit()
        {
            _unitOfWork.Commit();
        }

        public bool VerificarIntegracaoPrestadorRm(HorasMesPrestador horasMesPrestador)
        {
            var connectionStringRM = _connectionStrings.Value.RMConnection;
            int possuiRepresentante = 1, possuiEmpresa = 1;

            using (IDbConnection dbConnection = new SqlConnection(connectionStringRM))
            {
                var prestador = _prestadorRepository.Buscar(x => x.Id == horasMesPrestador.IdPrestador).FirstOrDefault();

                var codRepresentanteRM = prestador.IdRepresentanteRmTRPR;
                var valorString = ("000000" + codRepresentanteRM);
                var tam = valorString.Length;
                codRepresentanteRM = valorString.Substring(tam - 6);

                var codEmpresaRM = _empresaService.ObterCodEmpresaRm(horasMesPrestador.IdPrestador);

                var query = "SELECT count(1) from trpr where codrpr = '" + codRepresentanteRM + "'";
                possuiRepresentante = dbConnection.Query<int>(query).FirstOrDefault();

                query = "SELECT count(1) from fcfo where codcfo = '" + codEmpresaRM + "'";
                possuiEmpresa = dbConnection.Query<int>(query).FirstOrDefault();

                dbConnection.Close();
            }

            return possuiRepresentante + possuiEmpresa >= 2;
        }
    }
}