using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service.Interfaces;
using LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface;
using Logger.Context;
using Logger.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;
using Utils.Extensions;
using Utils.RM.Models;
using Utils.RM.Service;
using Utils.StfAnalitcsDW.Model;

namespace LancamentoFinanceiro.Domain.MigracaoRMRoot.Service
{
    public class MigracaoRMService : IMigracaoRMService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly ILancamentoFinanceiroService _lancamentoFinanceiroService;
        private readonly ITipoDespesaRepository _tipoDespesaRepository;
        private readonly ILancamentoFinanceiroRepository _lancamentoFinanceiroRepository;
        private readonly IServicoContratadoService _servicoContratadoService;
        private readonly IVariablesToken _variables;

        public MigracaoRMService(
            IOptions<ConnectionStrings> connectionStrings,
            MicroServicosUrls microServicosUrls,
            ILancamentoFinanceiroService lancamentoFinanceiroService,
            ITipoDespesaRepository tipoDespesaRepository,
            ILancamentoFinanceiroRepository lancamentoFinanceiroRepository,
            IServicoContratadoService servicoContratadoService, IVariablesToken variables)
        {
            _connectionStrings = connectionStrings;
            _microServicosUrls = microServicosUrls;
            _lancamentoFinanceiroService = lancamentoFinanceiroService;
            _tipoDespesaRepository = tipoDespesaRepository;
            _lancamentoFinanceiroRepository = lancamentoFinanceiroRepository;
            _servicoContratadoService = servicoContratadoService;
            _variables = variables;
        }

        public List<LancamentoFinanceiroRMDTO> BuscarLancamentosFinanceirosRM(string dtInicio, string dtFim)
        {
            var lancamentoFinanceiroRM = new RMBaixaService(_connectionStrings.Value.RMConnection);

            var lancamentos = lancamentoFinanceiroRM.ObterLancamentosFinanceirosRM(dtInicio, dtFim);
            return lancamentos;
        }

        public void MigrarLancamentosFinanceirosRM(IEnumerable<RootLancamentoFinanceiro> lancamentos)
        {
            var lancamentosAgrupados = lancamentos.GroupBy(x => new { x.CodigoColigada, x.IdLan });
            var lancamentosCorretosAgrupados = SplitList(VerificarValoresLancamentoFinanceiroAgrupados(lancamentosAgrupados));
            PersistirLancamentos(lancamentosCorretosAgrupados);
        }

        private List<RootLancamentoFinanceiro> VerificarValoresLancamentoFinanceiroAgrupados(IEnumerable<IGrouping<dynamic, RootLancamentoFinanceiro>> rootLancamentos)
        {
            var comprasEacessoService = new ComprasEacessoService(_connectionStrings.Value.EacessoConnection);
            var servicosEacessoService = new ClienteServicoEacessoService(_connectionStrings.Value.EacessoConnection);
            var lancamentosResult = new List<RootLancamentoFinanceiro>();

            foreach (var lancamento in rootLancamentos)
            {
                var lancamentoAgrupado = lancamento.FirstOrDefault();
                var itensLancamento = lancamento.SelectMany(x => x.ItensLancamentoFinanceiro).ToList();
                lancamentoAgrupado.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiro>();
                ObterTipoDespesaPorColigada(comprasEacessoService, lancamentoAgrupado);
                if (lancamentoAgrupado.IdTipoDespesa == 0)
                {
                    lancamentoAgrupado.IdTipoDespesa = null;
                }
                lancamentoAgrupado.LgUsuario = "EACESSO";

                foreach (var itemLancamento in itensLancamento)
                {
                    try
                    {
                        PrepararItemLancamento(servicosEacessoService, lancamentoAgrupado, itemLancamento);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
                
                lancamentosResult.Add(lancamentoAgrupado);

                if (lancamentosResult.Count % 200 == 0)
                {
                    Console.WriteLine("Lancamentos RM Montados: " + lancamentosResult.Count + " - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                }
            }

            return lancamentosResult;
        }

        private void PersistirLancamentos(List<List<RootLancamentoFinanceiro>> lancamentosCorretos)
        {
            foreach (var lancamentos in lancamentosCorretos)
            {
                _lancamentoFinanceiroService.AdicionarRange(lancamentos);
            }
        }

        private void PrepararItemLancamento(ClienteServicoEacessoService servicosEacessoService, RootLancamentoFinanceiro lancamentoAgrupado, ItemLancamentoFinanceiro itemLancamento)
        {
            if (lancamentoAgrupado.IdLan.HasValue)
            {
                var idServico = itemLancamento.IdServicoContratado;

                if (itemLancamento.CodigoCusto != null && itemLancamento.CodigoCusto.Any())
                {
                    var servicoPeloCodigo = itemLancamento.CodigoCusto.Split('.');
                    itemLancamento.IdServicoContratado = int.Parse(servicoPeloCodigo[2]);
                    idServico = itemLancamento.IdServicoContratado;
                }

                itemLancamento.IdServicoContratado =
                    _lancamentoFinanceiroRepository.ObterIdServicoContratado(itemLancamento.IdServicoContratado.Value);
                if (itemLancamento.IdServicoContratado == 0)
                {
                    if (idServico == 0)
                    {
                        return;
                    }
                    else
                    {
                        var novoServico = ObterServicoEacesso(idServico.Value, servicosEacessoService);
                        itemLancamento.IdServicoContratado = _servicoContratadoService.PersistirServicoEacesso(novoServico);
                    }
                }
                lancamentoAgrupado.ItensLancamentoFinanceiro.Add(itemLancamento);
            }
            else
            {
                AdicionarLogGenerico("ERRO LANCAMENTO - dtBaixa: " + lancamentoAgrupado.DtBaixa + " - vlBaixado: " + lancamentoAgrupado.VlBaixado + " - codcoligada: " + lancamentoAgrupado.CodigoColigada + " - idlan: " + lancamentoAgrupado.IdLan, JsonConvert.SerializeObject(lancamentoAgrupado));
            }
        }

        private void ObterTipoDespesaPorColigada(ComprasEacessoService comprasEacessoService, RootLancamentoFinanceiro lancamentoAgrupado)
        {
            if (lancamentoAgrupado.DescricaoOrigemLancamento != "FT")
            {
                var tipoDespesa = comprasEacessoService.ObterTipoDespesaPorIdColigadaIdLan(int.Parse(lancamentoAgrupado.CodigoColigada), lancamentoAgrupado.IdLan.Value);
                if (tipoDespesa != null)
                {
                    lancamentoAgrupado.IdTipoDespesa = _tipoDespesaRepository.ObterTipoDespesaPorSigla(tipoDespesa.TipoDespesa);
                    lancamentoAgrupado.DescOrigemCompraEacesso = tipoDespesa.OrigemEacesso;
                }
            }
        }

        private List<RootLancamentoFinanceiro> VerificarValoresLancamentoFinanceiro(List<RootLancamentoFinanceiro> rootLancamentos)
        {
            _variables.UserName = "RM";
            var comprasEacessoService = new ComprasEacessoService(_connectionStrings.Value.EacessoConnection);
            var notasEacessoService = new NotasFiscaisEacessoService(_connectionStrings.Value.EacessoConnection);
            var servicosEacessoService = new ClienteServicoEacessoService(_connectionStrings.Value.EacessoConnection);
            var lancamentosResult = new List<RootLancamentoFinanceiro>();

            foreach (var lancamento in rootLancamentos)
            {
                try
                {
                    if (lancamento.IdLan.HasValue)
                    {
                        var item = lancamento.ItensLancamentoFinanceiro.FirstOrDefault();
                        var idServico = item.IdServicoContratado;

                        if (item.CodigoCusto != null && item.CodigoCusto.Any())
                        {
                            var servicoPeloCodigo = item.CodigoCusto.Split('.');
                            item.IdServicoContratado = int.Parse(servicoPeloCodigo[2]);
                            idServico = item.IdServicoContratado;
                        }

                        item.IdServicoContratado = ObterServicoContratadoPorIdServicoEacesso(item.IdServicoContratado.Value);
                        item.IdServicoContratado =
                            _lancamentoFinanceiroRepository.ObterIdServicoContratado(item.IdServicoContratado.Value);
                        if (item.IdServicoContratado == 0)
                        {
                            if (idServico == 0)
                            {
                                continue;
                            }
                            else
                            {
                                var novoServico = ObterServicoEacesso(idServico.Value, servicosEacessoService);
                                VerificarTipoDespesa(comprasEacessoService, lancamento, item);
                                item.IdServicoContratado = _servicoContratadoService.PersistirServicoEacesso(novoServico);
                            }
                        }
                        else
                        {
                            VerificarTipoDespesa(comprasEacessoService, lancamento, item);
                        }

                        lancamento.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiro> { item };
                        if (lancamento.IdTipoDespesa == 0)
                        {
                            lancamento.IdTipoDespesa = null;
                        }
                        lancamento.LgUsuario = "EACESSO";
                        lancamentosResult.Add(lancamento);
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            return lancamentosResult;
        }

        private void VerificarTipoDespesa(ComprasEacessoService comprasEacessoService, RootLancamentoFinanceiro lancamento, ItemLancamentoFinanceiro item)
        {
            if (lancamento.DescricaoOrigemLancamento != "FT")
            {
                var tipoDespesa = comprasEacessoService.ObterTipoDespesaPorIdServicoIdLan(item.IdServicoContratado.Value, lancamento.IdLan.Value);
                if (tipoDespesa == null)
                {
                    tipoDespesa = comprasEacessoService.ObterTipoDespesaPorIdColigadaIdLan(int.Parse(lancamento.CodigoColigada), lancamento.IdLan.Value);
                }
                if (tipoDespesa != null)
                {
                    lancamento.IdTipoDespesa = _tipoDespesaRepository.ObterTipoDespesaPorSigla(tipoDespesa.TipoDespesa);
                    lancamento.DescOrigemCompraEacesso = tipoDespesa.OrigemEacesso;
                }
            }
        }
        
        private int VerificarIdServicoLancamentosCompra(int idlan, int idColigada, ComprasEacessoService comprasEacessoService)
        {
            var result = comprasEacessoService.ObterIdServicoidClienteIdCelulaPorIdLanIdColigada(idlan, idColigada);
            if (result != null && result.IdServico.HasValue)
            {
                return result.IdServico.Value;
            }
            return 0;
        }

        private int VerificarIdServicoLancamentosFaturamento(int idMov, int idColigada, NotasFiscaisEacessoService notasEacessoService)
        {
            var result = notasEacessoService.ObterIdServicoidClienteIdCelulaPorIdMovIdColigada(idMov, idColigada);
            if (result != null && result.IdServico.HasValue)
            {
                return result.IdServico.Value;
            }
            return 0;
        }

        private int ObterServicoContratadoPorIdServicoEacesso(int idServicoEacesso)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);

            client.BaseAddress = new Uri(_microServicosUrls.UrlApiServico);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/de-para/" + idServicoEacesso + "/obter-servico-contratado-por-servico-eacesso").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Int32.Parse(responseString);
        }
        
        private ViewServicoModel ObterServicoEacesso(int idServicoEacesso, ClienteServicoEacessoService servicoEacessoService)
        {
            return servicoEacessoService.ObterServicoPorId(idServicoEacesso);
        }

        public static List<List<RootLancamentoFinanceiro>> SplitList(List<RootLancamentoFinanceiro> lancamentos, int nSize = 2000)
        {
            var list = new List<List<RootLancamentoFinanceiro>>();

            for (int i = 0; i < lancamentos.Count; i += nSize)
            {
                list.Add(lancamentos.GetRange(i, Math.Min(nSize, lancamentos.Count - i)));
            }

            return list;
        }

        private static void AdicionarLogGenerico(string descricaoLog, string stackTrace = "")
        {
            var _logContext = new LogGenericoContext();
            _logContext.LogGenericos.Add(new LogGenerico
            {
                NmTipoLog = Tipo.INTEGRACAO.GetDescription(),
                NmOrigem = Origem.SALESFORCE.GetDescription(),
                DtHoraLogGenerico = DateTime.Now,
                DescLogGenerico = descricaoLog,
                DescExcecao = stackTrace
            });
            _logContext.SaveChanges();
        }
    }
}

internal class ChaveColigada
{
    public int CodigoColigada { get; set; }
    public int? IdLan { get; set; }
}
