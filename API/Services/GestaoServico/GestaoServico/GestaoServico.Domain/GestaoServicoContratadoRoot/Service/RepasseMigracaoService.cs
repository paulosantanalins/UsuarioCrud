using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;
using Logger.Context;
using Logger.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Models;
using Utils.EacessoLegado.Service;
using Utils.Extensions;
using Utils.StfAnalitcsDW.Model;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service
{
    public class RepasseMigracaoService : IRepasseMigracaoService
    {
        private readonly IDeParaServicoService _deParaServicoService;
        private readonly IServicoContratadoService _servicoContratadoService;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IRepasseRepository _repasseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public RepasseMigracaoService(
            IDeParaServicoService deParaServicoService,
            IServicoContratadoService servicoContratadoService,
            IRepasseRepository repasseRepository,
            MicroServicosUrls microServicosUrls,
            IUnitOfWork unitOfWork,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _deParaServicoService = deParaServicoService;
            _servicoContratadoService = servicoContratadoService;
            _repasseRepository = repasseRepository;
            _microServicosUrls = microServicosUrls;
            _unitOfWork = unitOfWork;
            _connectionStrings = connectionStrings;
        }

        public void MigrarRepassesEacesso(List<Repasse> repasses)
        {
            PersistirRepasses(repasses);
        }

        public List<EAcessoRepasse> BuscarRepassesEAcesso(string dtInicio, string dtFim)
        {
            var repassesEAcessoService = new EAcessoRepasseService(_connectionStrings.Value.EacessoConnection);
            var repassesEAcesso = repassesEAcessoService.ObterRepassesEAcesso(dtInicio, dtFim);
            Console.WriteLine("Total de repasses: " + repassesEAcesso.Count + " - Time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            return repassesEAcesso;
        }

        private int PersistirRepasses(List<Repasse> repasses)
        {
            var idsPersistidos = new List<ControleRepasseMae>();
            var repassesPronto = new List<Repasse>();

            foreach (var repasse in repasses)
            {
                repasse.Id = 0;
                var repasseEacesso = PersistirRepasseAtual(repasse);
                if (repasseEacesso != null)
                {
                    repassesPronto.Add(repasseEacesso);
                }
                if ((repassesPronto.Count % 500) == 0)
                {
                    Console.WriteLine("Total de repasses montados: " + repassesPronto.Count + " - Time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                }
            }
            Console.WriteLine("Start persistencia - Time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            repassesPronto = BulkInsert(SplitList(repassesPronto));
            RealizarPersistenciaLancamentosFinanceiros(repassesPronto);
            return repassesPronto.Count;
        }

        private Repasse PersistirRepasseAtual(Repasse repasseAtual)
        {
            //DESCOMENTAR PARA POPULAR COM AS INFORMAÇÕES DE DESPESA
            //var repasseDb = _repasseRepository.Buscar(x => x.IdRepasseEacesso == repasseAtual.IdRepasseEacesso).FirstOrDefault();
            //if (repasseDb != null)
            //{
            //    repasseDb.VlInc = repasseAtual.VlInc;
            //    repasseDb.VlDesc = repasseAtual.VlDesc;
            //    repasseDb.IdTipoDespesa = repasseAtual.IdTipoDespesa;
            //    return repasseDb;
            //}
            var servicosEacessoService = new ClienteServicoEacessoService(_connectionStrings.Value.EacessoConnection);
            try
            {
                repasseAtual.IdRepasseMae = null;
                repasseAtual.DtRepasseMae = null;

                var idOrigem = _deParaServicoService.BuscarIdServicoContratadoPorIdServicoEacesso(repasseAtual.IdServicoContratadoOrigem);
                var idDestino = _deParaServicoService.BuscarIdServicoContratadoPorIdServicoEacesso(repasseAtual.IdServicoContratadoDestino);
                if (idOrigem != 0 && idDestino != 0 && (idOrigem != 2233 || idDestino != 2233))
                {
                    if (idOrigem != 0)
                        repasseAtual.IdServicoContratadoOrigem = idOrigem;
                    else
                    {
                        var servicoEacesso = ObterServicoEacesso(repasseAtual.IdServicoContratadoOrigem, servicosEacessoService);
                        var servico = ViewServicoVmParaServicoContratado(servicoEacesso);
                        repasseAtual.IdServicoContratadoOrigem = _servicoContratadoService.PersistirServicoEacesso(servico, servicoEacesso.IdServico, servicoEacesso.NomeServico, servicoEacesso.IdCliente, servicoEacesso.Markup, servicoEacesso.IdContrato, servicoEacesso.SiglaTipoServico, servicoEacesso.DescEscopo, servicoEacesso.SubTipo);

                    }

                    if (idDestino != 0)
                        repasseAtual.IdServicoContratadoDestino = idDestino;
                    else
                    {
                        var servicoEacesso = ObterServicoEacesso(repasseAtual.IdServicoContratadoDestino, servicosEacessoService);
                        var servico = ViewServicoVmParaServicoContratado(servicoEacesso);
                        repasseAtual.IdServicoContratadoDestino = _servicoContratadoService.PersistirServicoEacesso(servico, servicoEacesso.IdServico, servicoEacesso.NomeServico, servicoEacesso.IdCliente, servicoEacesso.Markup, servicoEacesso.IdContrato, servicoEacesso.SiglaTipoServico, servicoEacesso.DescEscopo, servicoEacesso.SubTipo);
                    }
                    return repasseAtual;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                AdicionarLogGenerico(string.Format("Ocorreu um erro na migracao do repasse eacesso {0}", JsonConvert.SerializeObject(repasseAtual)), e.StackTrace);
                return null;
            }
        }

        private ServicoContratado ViewServicoVmParaServicoContratado(ViewServicoModel src)
        {
            var dest = new ServicoContratado
            {
                DtFinal = src.DtFimVigencia ?? DateTime.Now,
                DtInicial = src.DtInicioVigencia,
                DescTipoCelula = src.DescTipoCelula == "COMERCIAL" ? "COM" : "TEC",
                FlReembolso = src.FlReembolso == 1 ? true : false,
                QtdExtraReembolso = src.QtdExtraReembolso ?? 0,
                NmTipoReembolso = src.NmTipoReembolso == 0 ? null : src.NmTipoReembolso == 1 ? "Nota de serviço" : "Nota de débito",
                FlHorasExtrasReembosaveis = src.FlHorasExtrasReembolsaveis == 1 ? true : false,
                VlRentabilidade = src.VlRentabilidadePrevista,
                FlReoneracao = src.FlReoneracao == 1 ? true : false,
                FlFaturaRecorrente = src.FlFaturaRecorrente == 1 ? true : false,
                IdProdutoRM = src.NrProdutoRM,
                FormaFaturamento = src.DescFormaFaturamento
            };
            return dest;
        }

        public List<Repasse> BulkInsert(List<List<Repasse>> repassesList)
        {
            var repassesResult = new List<Repasse>();
            int count = 0;
            foreach (var repasses in repassesList)
            {
                _repasseRepository.AdicionarRange(repasses);
                _unitOfWork.Commit();
                repassesResult.AddRange(repasses);

                count += repasses.Count;
                Console.WriteLine("Total de repasses persistidos: " + count + " - Time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            }
            return repassesResult;
        }

        private bool RealizarPersistenciaLancamentosFinanceiros(List<Repasse> repasses)
        {
            var lancamentos = GerarLancamentosFinanceiros(repasses);
            Console.WriteLine("Start lancamentos financeiros" + " - Time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            foreach (var lancamentosValidos in lancamentos)
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(_microServicosUrls.UrlApiLancamentoFinanceiro);
                client.Timeout = TimeSpan.FromHours(3);
                Persistir(client, lancamentosValidos);
            }
            return true;
        }

        private static void Persistir(HttpClient client, List<LancamentoFinanceiroRepasseDto> lancamentosValidos)
        {
            client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("usuario", "EPM");
            var content = new StringContent(JsonConvert.SerializeObject(lancamentosValidos), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/LancamentoFinanceiro/gerar-credito-debito", content).Result;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Falha ao persistir repasses" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                var idsRepasses = lancamentosValidos.Select(x => x.ItensLancamentoFinanceiro.FirstOrDefault());
                Console.WriteLine("Falha com os repasses entre: " + string.Join(", ", idsRepasses.Select(x => x.IdRepasse)));
            }
        }

        private static List<List<LancamentoFinanceiroRepasseDto>> GerarLancamentosFinanceiros(List<Repasse> repasses)
        {
            var lancamentos = new List<LancamentoFinanceiroRepasseDto>();
            foreach (var repasse in repasses)
            {
                var lancamentoBase = new LancamentoFinanceiroRepasseDto
                {
                    DtBaixa = repasse.DtRepasse,
                    DtLancamento = DateTime.Now.Date,
                    CodigoColigada = null,
                    DescricaoOrigemLancamento = "RP",
                    IdLan = null,
                    IdTipoDespesa = repasse.IdTipoDespesa,
                    LgUsuario = "Eacesso",
                    DescOrigemCompraEacesso = null
                };

                var lancamentoCredito = (LancamentoFinanceiroRepasseDto)lancamentoBase.Clone();
                var lancamentoDebito = (LancamentoFinanceiroRepasseDto)lancamentoBase.Clone();

                lancamentoCredito.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiroRepasseDto> {
                    new ItemLancamentoFinanceiroRepasseDto
                    {
                        DtRepasse = repasse.DtRepasse,
                        IdRepasse = repasse.Id,
                        //verificar
                        VlLancamento = repasse.VlTotal.HasValue ? repasse.VlTotal.Value : 0,
                        IdServicoContratado = null,
                        VlInc = repasse.VlInc,
                        VlDesc = repasse.VlDesc,
                        LgUsuario = "Eacesso"
                    }
                };
                lancamentoCredito.DescricaoTipoLancamento = "C";

                lancamentoDebito.ItensLancamentoFinanceiro = new List<ItemLancamentoFinanceiroRepasseDto> {
                    new ItemLancamentoFinanceiroRepasseDto
                    {
                        DtRepasse = repasse.DtRepasse,
                        IdRepasse = repasse.Id,
                        //verificar
                        VlLancamento = repasse.VlTotal.HasValue ? repasse.VlTotal.Value : 0,
                        IdServicoContratado = null,
                        VlInc = repasse.VlInc,
                        VlDesc = repasse.VlDesc,
                        LgUsuario = "Eacesso"
                    }
                };
                lancamentoDebito.DescricaoTipoLancamento = "D";


                lancamentos.AddRange(new List<LancamentoFinanceiroRepasseDto> { lancamentoDebito, lancamentoCredito });
            }

            return SplitList(lancamentos);
        }

        private static void AdicionarLogGenerico(string descricaoLog, string stackTrace = "")
        {
            var _logContext = new LogGenericoContext();
            _logContext.LogGenericos.Add(new LogGenerico
            {
                NmTipoLog = Tipo.INTEGRACAO.GetDescription(),
                NmOrigem = Origem.EPM.GetDescription(),
                DtHoraLogGenerico = DateTime.Now,
                DescLogGenerico = descricaoLog,
                DescExcecao = stackTrace
            });
            _logContext.SaveChanges();
        }

        private ViewServicoModel ObterServicoEacesso(int idServicoEacesso, ClienteServicoEacessoService servicoEacessoService)
        {
            return servicoEacessoService.ObterServicoPorId(idServicoEacesso);
        }

        public static List<List<LancamentoFinanceiroRepasseDto>> SplitList(List<LancamentoFinanceiroRepasseDto> lancamentos, int nSize = 200)
        {
            var list = new List<List<LancamentoFinanceiroRepasseDto>>();

            for (int i = 0; i < lancamentos.Count; i += nSize)
            {
                list.Add(lancamentos.GetRange(i, Math.Min(nSize, lancamentos.Count - i)));
            }

            return list;
        }

        public static List<List<Repasse>> SplitList(List<Repasse> lancamentos, int nSize = 3000)
        {
            var list = new List<List<Repasse>>();

            for (int i = 0; i < lancamentos.Count; i += nSize)
            {
                list.Add(lancamentos.GetRange(i, Math.Min(nSize, lancamentos.Count - i)));
            }

            return list;
        }
    }

    internal class ControleRepasseMae
    {
        public int IdEacesso { get; set; }
        public int IdStfCorp { get; set; }
    }

}
