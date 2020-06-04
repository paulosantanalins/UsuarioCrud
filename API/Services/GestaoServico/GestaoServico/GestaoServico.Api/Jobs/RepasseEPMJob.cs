using AutoMapper;
using FluentScheduler;
using GestaoServico.Api.ViewModels.Lancamento;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Infra.CrossCutting.IoC;
using GestaoServico.Infra.Data.SqlServer.Context;
using Logger.Context;
using Logger.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using Utils.EacessoLegado.Models;
using Utils.EacessoLegado.Service;
using Utils.Email;
using Utils.Extensions;
using Utils.StfAnalitcsDW.Model;

namespace GestaoServico.Api.Jobs
{
    public class RepasseEPMJob : IJob
    {
        private static HttpClient client = new HttpClient();
        private StringBuilder sb = new StringBuilder();
        const string EACESSO_DB = "STFCORP_FENIX_EPM2105";

        public void Execute()
        {
            try
            {

                CriarConnection();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
        public void CriarConnection()
        {
            var eacessoConnection = new EpmRepasseService("Data Source=10.161.69.101\\CORP_H;Initial Catalog=" + EACESSO_DB + ";User ID=stfcorp_fenix;Password=\"noRPH|dt*;\"");

            var repassesInternos = new List<EpmRepasse>();
            var repassesComerciais = new List<EpmRepasse>();
            var repassesEpm = eacessoConnection.ObterRepasse();
            
            VerificarTiposRepasse(repassesInternos, repassesComerciais, repassesEpm);

            var repassesFormatoCorretoInterno = Mapper.Map<List<Repasse>>(repassesInternos);
            var repassesFormatoCorretoComercial = Mapper.Map<List<Repasse>>(repassesComerciais);
            var count = 0;
            count = PersistirRepasses(repassesFormatoCorretoInterno, count, true);
            count = PersistirRepasses(repassesFormatoCorretoComercial, count, false);
            
            EnviarEmailConfirmacaoEnvio(null, count);
        }

        private static void VerificarTiposRepasse(List<EpmRepasse> repassesInternos, List<EpmRepasse> repassesComerciais, List<EpmRepasse> repassesEpm)
        {
            var eacessoConnectionCelula = new CelulaEacessoService("Data Source=10.161.69.101\\CORP_H;Initial Catalog=" + EACESSO_DB + ";User ID=stfcorp_fenix;Password=\"noRPH|dt*;\"");
            foreach (var repasse in repassesEpm)
            {
                if (!repasse.GROUPED && (repasse.IdServicoProf != null) && repasse.LancRepInterno && (repasse.IdTipoCelTec == 3 || repasse.IdTipoCelTec == 9))
                {
                    AdicionarRepasseInterno(repassesInternos, (EpmRepasse)repasse.Clone(), eacessoConnectionCelula);
                }
                if (repasse.Checked)
                {
                    AdicionarRepasseComercial(repassesComerciais, (EpmRepasse)repasse.Clone(), eacessoConnectionCelula);
                }
                
            }
        }

        private int PersistirRepasses(List<Repasse> repassesFormatoCorreto, int count, bool tipoRepasse)
        {
            
            var serviceSevicoContratado = RecuperarImportacaoServicoContratadoService();
            var serviceSevicoContratadoDePara = RecuperarImportacaoServiceDePara();
            var servicosEacessoService = new ClienteServicoEacessoService("Data Source=10.161.69.101\\CORP_H;Initial Catalog=" + EACESSO_DB + ";User ID=stfcorp_fenix;Password=\"noRPH|dt*;\"");
            var variables = RecuperarVariablesToken();
            variables.UserName = "EPM";
            
            foreach (var repasse in repassesFormatoCorreto.ToList())
            {
                repasse.Id = 0;
                var _context = new GestaoServicoContext(variables);
                try
                {
                    repasse.FlRepasseInterno = tipoRepasse;
                    var idOrigem = serviceSevicoContratadoDePara.BuscarIdServicoContratadoPorIdServicoEacesso(repasse.IdServicoContratadoOrigem);
                    var idDestino = serviceSevicoContratadoDePara.BuscarIdServicoContratadoPorIdServicoEacesso(repasse.IdServicoContratadoDestino);
                    if (idOrigem != 0)
                        repasse.IdServicoContratadoOrigem = idOrigem;
                    else
                    {
                        var servicoEacesso = ObterServicoEacesso(repasse.IdServicoContratadoOrigem, servicosEacessoService);
                        var servico = Mapper.Map<ServicoContratado>(servicoEacesso);
                        repasse.IdServicoContratadoOrigem = serviceSevicoContratado.PersistirServicoEacesso(servico, servicoEacesso.IdServico, servicoEacesso.NomeServico, servicoEacesso.IdCliente, servicoEacesso.Markup, servicoEacesso.IdContrato, servicoEacesso.SiglaTipoServico, servicoEacesso.DescEscopo, servicoEacesso.SubTipo);

                    }

                    if (idDestino != 0)
                        repasse.IdServicoContratadoDestino = idDestino;
                    else
                    {
                        var servicoEacesso = ObterServicoEacesso(repasse.IdServicoContratadoDestino, servicosEacessoService);
                        var servico = Mapper.Map<ServicoContratado>(servicoEacesso);
                        repasse.IdServicoContratadoDestino = serviceSevicoContratado.PersistirServicoEacesso(servico, servicoEacesso.IdServico, servicoEacesso.NomeServico, servicoEacesso.IdCliente, servicoEacesso.Markup, servicoEacesso.IdContrato, servicoEacesso.SiglaTipoServico, servicoEacesso.DescEscopo, servicoEacesso.SubTipo);
                    }
                    //var repasseExistente = _context.Repasses.FirstOrDefault(x => x.IdEpm == repasse.IdEpm);

                    _context.Add(repasse);
                    _context.SaveChanges();
                    _context.DetachAllEntities();
                    if (repasse.FlStatus == "AP")
                    {
                        if (!RealizarPersistenciaLancamentosFinanceiros(repasse))
                        {
                            _context.Remove(repasse);
                            _context.SaveChanges();
                            _context.DetachAllEntities();
                            AdicionarLogGenerico("Removido repasse com id: " + repasse.Id);
                            continue;
                        }
                    }
                    count++;
                }
                catch (Exception e)
                {
                    _context.DetachAllEntities();
                    AdicionarLogGenerico(string.Format("Ocorreu um erro ao persistir o repasse {0} - {1}", repasse.IdEpm, repasse.DescProjeto), e.StackTrace);
                    continue;
                }
            }
            return count;
        }

        private static List<LancamentoFinanceiroRepasseVM> GerarLancamentosFinanceiros(Repasse repasse)
        {
            var lancamentoBase = new LancamentoFinanceiroRepasseVM
            {
                DtBaixa = repasse.DtRepasse,
                DtLancamento = DateTime.Now.Date,
                CodigoColigada = null,
                DescricaoOrigemLancamento = "RP",
                IdLan = null,
                IdTipoDespesa = 13,
                LgUsuario = "EPM",
                DtAlteracao = DateTime.Now
            };

            var lancamentoCredito = (LancamentoFinanceiroRepasseVM)lancamentoBase.Clone();
            var lancamentoDebito = (LancamentoFinanceiroRepasseVM)lancamentoBase.Clone();

            lancamentoCredito.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiroRepasseVM> {
                new ItemLancamentoFinanceiroRepasseVM
                {
                    DtRepasse = repasse.DtRepasse,
                    IdRepasse = repasse.Id,
                    //verificar
                    VlLancamento = repasse.VlTotal.HasValue ? repasse.VlTotal.Value : 0,
                    IdServicoContratado = repasse.IdServicoContratadoDestino
                }
            };
            lancamentoCredito.DescricaoTipoLancamento = "C";

            lancamentoDebito.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiroRepasseVM> {
                new ItemLancamentoFinanceiroRepasseVM
                {
                    DtRepasse = repasse.DtRepasse,
                    IdRepasse = repasse.Id,
                    //verificar
                    VlLancamento = repasse.VlTotal.HasValue ? repasse.VlTotal.Value : 0,
                    IdServicoContratado = repasse.IdServicoContratadoOrigem
                }
            };
            lancamentoDebito.DescricaoTipoLancamento = "D";
            return new List<LancamentoFinanceiroRepasseVM> { lancamentoDebito, lancamentoCredito };

        }

        private static bool RealizarPersistenciaLancamentosFinanceiros(Repasse repasse)
        {
            var lancamentos = GerarLancamentosFinanceiros(repasse);
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromHours(3);

            var _microservicoUrls = RecuperarMicroServicosUrls();
            client.BaseAddress = new Uri(_microservicoUrls.UrlApiLancamentoFinanceiro);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("usuario", "EPM");
            var content = new StringContent(JsonConvert.SerializeObject(lancamentos), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/LancamentoFinanceiro/gerar-credito-debito", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AdicionarLogGenerico("Erro ao gerar lançamentos financeiros.", response.Content.ReadAsStringAsync().Result);
                return false;
            }
            else
            {
                AdicionarLogGenerico("Realizado a geração de lançamentos de crédito e débito.");
                return true;
            }
        }

        private static void AdicionarRepasseComercial(List<EpmRepasse> repassesComerciais, EpmRepasse repasse, CelulaEacessoService eacessoConnectionCelula)
        {
            repasse.FlStatus = "NA";
            if (repasse.Comments == null)
            {
                repasse.Comments = "";
            }
            var novoComent = repasse.IdRepasse != null ? repasse.IdRepasse.Value.ToString() + " - " : "";

            repasse.Comments = novoComent + "Gerado pelo Modulo de Automacao de Horas - EPM COMERCIAL - " + repasse.Comments;
            var idtecnico = repasse.IdServicoTec;
            if (int.TryParse(repasse.IdServicoCom, out int result))
            {
                repasse.IdServicoTec = int.Parse(repasse.IdServicoCom);
                repasse.IdServicoCom = idtecnico.ToString();
                repasse.IdMoeda = eacessoConnectionCelula.ObterMoedaCelula(repasse.IdCelulaTec);
                repassesComerciais.Add(repasse);
            }
            else
            {
                repasse.IdServicoTec = idtecnico;
                AdicionarLogGenerico("EPM COMERCIAL ERROR - repasse com id:" + repasse.Id + "não possui servico tecnico", "");
            }
        }

        private static void AdicionarRepasseInterno(List<EpmRepasse> repassesInternos, EpmRepasse repasse, CelulaEacessoService eacessoConnectionCelula)
        {
            if (repasse.Comments == null)
            {
                repasse.Comments = "";
            }
            var novoComent = repasse.IdRepasseInterno != null ? repasse.IdRepasseInterno.Value.ToString() + " - " : "";
            var comment = repasse.Comments;
            repasse.Comments = novoComent + "Gerado pelo Modulo de Automacao de Horas - EPM INTERNO - " + repasse.Comments;
            if (repasse.IdCelulaProf != repasse.IdCelulaTec)
            {
                repasse.FlStatus = "NA";
            }
            else
            {
                repasse.FlStatus = "AP";
            }

            repasse.IdMoeda = eacessoConnectionCelula.ObterMoedaCelula(repasse.IdCelulaProf);

            repasse.TransferWork = 1;
            repasse.TransferRate = repasse.ActualCost;
            repasse.TransferCost = repasse.ActualCost;
            repasse.IdServicoCom = repasse.IdServicoProf.Value.ToString();

            repassesInternos.Add(repasse);
        }

        public void EnviarEmailConfirmacaoEnvio(List<EpmRepasse> epmRepasses = null, int count = 0)
        {
            sb = new StringBuilder();
            var emailGenerico = new EmailGenericoModel();

            emailGenerico.Assunto = "Integração EPM";
            emailGenerico.RemetenteNome = "STF-CORP";
            emailGenerico.Para = "masilva21@stefanini.com,mggeraldo@stefanini.com,emlopes1@stefanini.com,clbatista@stefanini.com";
            //emailGenerico.Para = "emlopes1@stefanini.com";
            MontarCorpoEmail(emailGenerico, epmRepasses, count);
            RealizarEnvioEmail(emailGenerico);
        }

        private void RealizarEnvioEmail(EmailGenericoModel emailGenerico)
        {
            var client = new HttpClient();
            var _microservicoUrls = RecuperarMicroServicosUrls();
            client.BaseAddress = new Uri(_microservicoUrls.UrlEnvioEmail);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(emailGenerico), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/Email", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private void MontarCorpoEmail(EmailGenericoModel emailGenerico, List<EpmRepasse> epmRepasses = null, int count = 0)
        {
            sb.Append(string.Format("<html><body>"));
            sb.Append(string.Format("Foram persistidos com sucesso {0}, repasses no dia {1}", count, DateTime.Now.ToString("yyyy-MM-dd")));
            sb.Append(string.Format("</body></html>"));
            emailGenerico.Corpo = sb.ToString();
        }

        private static void AdicionarLogGenerico(string descricaoLog, string stackTrace = "")
        {
            var _logContext = new LogGenericoContext();
            _logContext.LogGenericos.Add(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.EPM.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = descricaoLog, DescExcecao = stackTrace });
            _logContext.SaveChanges();
        }

        private static IDeParaServicoService RecuperarImportacaoServiceDePara()
        {
            var importacaoService = Injector.ServiceProvider.GetService(typeof(IDeParaServicoService)) as IDeParaServicoService;
            return importacaoService;
        }

        private static IServicoContratadoService RecuperarImportacaoServicoContratadoService()
        {
            var importacaoService = Injector.ServiceProvider.GetService(typeof(IServicoContratadoService)) as IServicoContratadoService;
            return importacaoService;
        }

        private static MicroServicosUrls RecuperarMicroServicosUrls()
        {
            var microServicosUrls = Injector.ServiceProvider.GetService(typeof(MicroServicosUrls)) as MicroServicosUrls;
            return microServicosUrls;
        }

        private ViewServicoModel ObterServicoEacesso(int idServicoEacesso, ClienteServicoEacessoService servicoEacessoService)
        {
            return servicoEacessoService.ObterServicoPorId(idServicoEacesso);
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
