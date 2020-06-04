using System;
using System.Collections.Generic;
using System.Linq;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Cadastro.Domain.EmailRoot.DTO;
using System.Net.Http;
using Utils.Base;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using Novell.Directory.Ldap;
using Utils;
using Utils.Extensions;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class ReajusteContratoService : IReajusteContratoService
    {
        private readonly IReajusteContratoRepository _reajusteContratoRepository;
        private readonly IPrestadorService _prestadorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly LinkBase _linkBase;
        private readonly LdapSeguranca[] _ldapConfig;

        public ReajusteContratoService(IReajusteContratoRepository reajusteContratoRepository, IUnitOfWork unitOfWork,
            IPrestadorService prestadorService, MicroServicosUrls microServicosUrls, LinkBase linkBase, LdapSeguranca[] ldapConfig)
        {
            _reajusteContratoRepository = reajusteContratoRepository;
            _unitOfWork = unitOfWork;
            _prestadorService = prestadorService;
            _microServicosUrls = microServicosUrls;
            _linkBase = linkBase;
            _ldapConfig = ldapConfig;
        }

        public IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastradas() =>
            _reajusteContratoRepository.BuscarPeriodosComFinalizacaoCadastrada();

        public FiltroComPeriodo<ReajusteContratoGridDto> Filtrar(FiltroComPeriodo<ReajusteContratoGridDto> filtro)
        {
            var result = _reajusteContratoRepository.Filtrar(filtro);
            return result;
        }

        public IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula)
        {
            var result = _reajusteContratoRepository.ObterPrestadoresPorCelula(idCelula);
            return result;
        }

        public ResponseReajusteContrato ObterValoresAtuais(int idPrestador, bool filtrar)
        {
            var prestador = _reajusteContratoRepository.ObterPrestadorParaReajuste(idPrestador, filtrar);

            if (prestador == null)
                return new ResponseReajusteContrato { Message = "Já existe uma solicitação de reajuste/finalização de contrato para o Prestador neste mês, favor verificar" };

            var valores = prestador.ValoresPrestador.FirstOrDefault(x =>
                x.DataReferencia == prestador.ValoresPrestador.Max(y => y.DataReferencia));

            if (valores == null) return new ResponseReajusteContrato { Message = "Este Prestador não possui Valores de Contrato" };

            return new ResponseReajusteContrato
            {
                ValoresContratoPrestadorModel = new ValoresContratoPrestadorModel
                {
                    Quantidade = valores.Quantidade,
                    Valor = valores.ValorMes,
                    TipoContrato = valores.TipoRemuneracao.DescricaoValor
                }
            };
        }

        public void ReajustarContrato(ReajusteContrato reajusteContrato)
        {
            reajusteContrato.DataInclusao = DateTime.Now;
            reajusteContrato.Situacao = SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP.GetHashCode();

            var prestador = _prestadorService.BuscarPorId(reajusteContrato.IdPrestador);
            // enviar emails

            EnviarEmailReajuste(reajusteContrato, prestador);
            CriarBroadcastReajuste(reajusteContrato);

            _reajusteContratoRepository.AdicionarComLog(reajusteContrato);

            _unitOfWork.Commit();
        }

        public IEnumerable<LogReajusteContrato> ObterLogsPorId(int id)
        {
            var logs = _reajusteContratoRepository.ObterLogsPorId(id);

            return logs;
        }

        public ValoresContratoPrestadorModel ConsultarReajuste(int id)
        {
            var result = _reajusteContratoRepository.ConsultarReajuste(id);
            return result;
        }

        public void AprovarReajuste(int id)
        {
            var reajuste = _reajusteContratoRepository.BuscarPorId(id);

            if (reajuste == null) throw new Exception();

            var acao = ObterAcaoLog(reajuste);

            reajuste.Situacao++;

            var prestador = _prestadorService.BuscarPorId(reajuste.IdPrestador);


            EnviarEmailReajuste(reajuste, prestador);
            CriarBroadcastReajuste(reajuste);

            _reajusteContratoRepository.UpdateComLog(reajuste, acao);

            _unitOfWork.Commit();
        }

        public void NegarReajute(InativacaoFinalizacaoContratoDto inativacao)
        {
            var reajuste = new ReajusteContrato
            {
                Id = inativacao.Id,
                Situacao = SharedEnuns.SituacoesReajusteContrato.ReajusteCancelado.GetHashCode()
            };

            var reajusteContrato = _reajusteContratoRepository.BuscarPorId(inativacao.Id);

            var prestador = _prestadorService.BuscarPorId(reajusteContrato.IdPrestador);

            EnviarEmailReajusteNegacao(reajuste, prestador, inativacao.Motivo);

            _reajusteContratoRepository.InativarFinalizacao(reajuste, inativacao.Motivo);

            _unitOfWork.Commit();
        }

        public void EfetuarReajustesDeContratos()
        {
            var reajustes = _reajusteContratoRepository.ObterReajustesParaJob();

            foreach (var reajuste in reajustes)
            {
                reajuste.Situacao = SharedEnuns.SituacoesReajusteContrato.ReajusteFinalizado.GetHashCode();

                // criar regristro na base e no eacesso
                var valorPrestador = new ValorPrestador
                {
                    DataAlteracao = DateTime.Now,
                    DataReferencia = reajuste.DataReajuste,
                    IdMoeda = 1,
                    IdPrestador = reajuste.IdPrestador,
                    IdTipoRemuneracao = reajuste.IdTipoContrato,
                    PermiteExcluir = false,
                    ValorMes = reajuste.ValorContrato,
                    ValorHora = reajuste.ValorContrato / reajuste.QuantidadeHorasContrato,
                    Quantidade = reajuste.QuantidadeHorasContrato
                };

                _reajusteContratoRepository.UpdateComLog(reajuste, SharedEnuns.AcoesLog.Finalizado.GetHashCode());

                _prestadorService.PersistirPrestadorRemuneracao(valorPrestador);
                _prestadorService.InserirRemuneracaoEAcesso(valorPrestador);
            }
        }

        private void EnviarEmailReajuste(ReajusteContrato reajusteContrato, Prestador prestador)
        {
            var parametros = DefinirParametrosEmail(reajusteContrato, prestador);

            var Destinatarios = "";

            switch (reajusteContrato.Situacao)
            {
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP:
                    Destinatarios = BuscarEmailsParaEnvio(
                            SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP.GetDescription(), prestador);
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoRemuneracao:
                    Destinatarios = BuscarEmailsParaEnvio(
                        SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoRemuneracao.GetDescription(), prestador);
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoContrtoladoria:
                    Destinatarios = BuscarEmailsParaEnvio(
                        SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoContrtoladoria.GetDescription(), prestador);
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel:
                    Destinatarios = BuscarEmailsParaEnvio(
                        SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel.GetDescription(), prestador);
                    break;
            }

            var email = new EmailDTO
            {
                IdTemplate = 11,
                RemetenteNome = "Gestão de Prestadores - Reajuste de Contrato",
                Para = ObterDestinatarioEmail(Destinatarios),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
        }

        private void EnviarEmailReajusteNegacao(ReajusteContrato reajusteContrato, Prestador prestador, string motivo)
        {
            var parametros = DefinirParametrosEmailNegacao(reajusteContrato, prestador, motivo);

            var Destinatarios = BuscarEmailsParaEnvioGerente(
                        SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel.GetDescription(), prestador);

            var email = new EmailDTO
            {
                IdTemplate = 12,
                RemetenteNome = "Gestão de Prestadores - Reajuste de Contrato",
                Para = ObterDestinatarioEmail(Destinatarios),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
        }

        private IEnumerable<ValorParametroEmailDTO> DefinirParametrosEmail(
                ReajusteContrato reajusteContrato, Prestador prestador)
        {
            var parametros = new List<ValorParametroEmailDTO>();

            var parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[NOMEPROFISSIONAL]",
                ParametroValor = prestador.Pessoa.Nome
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[DATACRIACAO]",
                ParametroValor = reajusteContrato.DataInclusao.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[NUMERONOMECELULA]",
                ParametroValor = prestador.IdCelula.ToString()
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[LINK]",
                ParametroValor = ObterLinkAprovacaoReajuste()
            };
            parametros.Add(parametro);

            return parametros;
        }

        private IEnumerable<ValorParametroEmailDTO> DefinirParametrosEmailNegacao(
                ReajusteContrato reajusteContrato, Prestador prestador, string motivo)
        {
            var parametros = new List<ValorParametroEmailDTO>();

            var parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[NOMEPROFISSIONAL]",
                ParametroValor = prestador.Pessoa.Nome
            };
            parametros.Add(parametro);

            parametro = new ValorParametroEmailDTO
            {
                ParametroNome = "[MOTIVO]",
                ParametroValor = motivo
            };
            parametros.Add(parametro);

            return parametros;
        }
        private string ObterDestinatarioEmail(string emailCorreto)
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = "bacachola@stefanini.com,gmguilherme@latam.stefanini.com,clbatista@stefanini.com";
                    break;
                case "STAGING":
                    destinatario = "bacachola@stefanini.com,gmguilherme@latam.stefanini.com,clbatista@stefanini.com,dchirata@stefanini.com";
                    break;
                case "PRODUCTION":
                    destinatario = emailCorreto;
                    break;
            }
            return destinatario;
        }
        private string ObterLinkAprovacaoReajuste()
        {
            var destinatario = "";
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario = $"{_linkBase.Development}stfcorp/gestao-de-terceiros/reajuste-de-contratos/aprovar";
                    break;
                case "STAGING":
                    destinatario = $"{_linkBase.Staging}stfcorp/gestao-de-terceiros/reajuste-de-contratos/aprovar";
                    break;
                case "PRODUCTION":
                    destinatario = $"{_linkBase.Production}stfcorp/gestao-de-terceiros/reajuste-de-contratos/aprovar";
                    break;
            }
            return destinatario;
        }
        private void EnviarEmail(EmailDTO email)
        {
            var client = new HttpClient { BaseAddress = new Uri(_microServicosUrls.UrlEnvioEmail) };

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/Email", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private void CriarBroadcastReajuste(ReajusteContrato reajuste)
        {

            var client = new HttpClient { BaseAddress = new Uri(_microServicosUrls.UrlApiControle) };


            var funcionalidade = ObterFuncionalidadeParaBroadcast(reajuste);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.PostAsync($"api/Broadcast/criar-broadcasts-reajuste-contrato/{funcionalidade}", null).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private static string ObterFuncionalidadeParaBroadcast(ReajusteContrato reajuste)
        {
            var funcionalidade = string.Empty;
            switch (reajuste.Situacao)
            {
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP:
                    funcionalidade = SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP.GetDescription();
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoRemuneracao:
                    funcionalidade = SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoRemuneracao.GetDescription();
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoContrtoladoria:
                    funcionalidade = SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoContrtoladoria.GetDescription();
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel:
                    funcionalidade = SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel.GetDescription();
                    break;
            }

            return funcionalidade;
        }

        private static int ObterAcaoLog(ReajusteContrato reajuste)
        {
            var acao = 0;
            switch (reajuste.Situacao)
            {
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoBP:
                    acao = SharedEnuns.AcoesLog.AprovacaoBP.GetHashCode();
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoRemuneracao:
                    acao = SharedEnuns.AcoesLog.AprovacaoRem.GetHashCode();
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoContrtoladoria:
                    acao = SharedEnuns.AcoesLog.AprovacaoControladoria.GetHashCode();
                    break;
                case (int)SharedEnuns.SituacoesReajusteContrato.AguardandoAprovacaoDiretoriaCel:
                    acao = SharedEnuns.AcoesLog.AprovacaoDirCel.GetHashCode();
                    break;
            }

            return acao;
        }

        private string BuscarEmailsParaEnvio(string funcionalidade, Prestador prestador)
        {
            var idFuncionalidade =
                _reajusteContratoRepository.ObterIdFuncionalidade(funcionalidade);

            var logins = _reajusteContratoRepository.ObterLoginsComFuncionalidade(idFuncionalidade, prestador);

            var listaEmails = new List<string>();
            using (var cn = new LdapConnection())
            {
                cn.Connect(_ldapConfig.First().Hostname, _ldapConfig.First().Port);
                cn.Bind("stefanini-dom\\almintegration", "stefanini@10");

                LdapSearchConstraints cons = cn.SearchConstraints;
                cons.ReferralFollowing = true;
                cn.Constraints = cons;

                foreach (var login in logins)
                {
                    var filterInformacaoUsuario = $"(sAMAccountName={login})";
                    var search = cn.Search("DC=stefanini,DC=dom", LdapConnection.SCOPE_SUB, filterInformacaoUsuario, null, false, (LdapSearchConstraints)null);

                    while (search.hasMore())
                    {
                        var nextEntry = search.next();
                        var user = nextEntry.getAttributeSet();

                        listaEmails.Add(user.getAttribute("mail").StringValue);
                        break;
                    }
                }
            }

            return string.Join(",", listaEmails);
        }
        private string BuscarEmailsParaEnvioGerente(string funcionalidade, Prestador prestador)
        {
            var idFuncionalidade =
                _reajusteContratoRepository.ObterIdFuncionalidade(funcionalidade);

            var logins = _reajusteContratoRepository.ObterEmailGerenteCelula(idFuncionalidade, prestador);

            var listaEmails = new List<string>();

            listaEmails.Add(logins.First());

            return string.Join(",", listaEmails);
        }
    }
}
