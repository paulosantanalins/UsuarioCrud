using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RepasseEAcesso.Domain.CelulaRoot.Entity;
using RepasseEAcesso.Domain.EmailRoot;
using RepasseEAcesso.Domain.EmailRoot.DTO;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Repository;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces;
using RepasseEAcesso.Domain.SharedRoot.Service.Interface;
using RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utils;
using Utils.Base;
using Utils.Calendario;
using Utils.Connections;

namespace RepasseEAcesso.Domain.PeriodoRepasseRoot.Services
{
    public class PeriodoRepasseService : IPeriodoRepasseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly Variables _variables;
        private readonly IPeriodoRepasseRepository _periodoRepasseRepository;
        private readonly ICalendarioService _calendarioService;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public PeriodoRepasseService(
            MicroServicosUrls microServicosUrls,
            Variables variables,
            ICalendarioService calendarioService,
            IOptions<ConnectionStrings> connectionStrings,
            IPeriodoRepasseRepository periodoRepasseRepository,
            IUnitOfWork unitOfWork
        )
        {
            _unitOfWork = unitOfWork;
            _variables = variables;
            _microServicosUrls = microServicosUrls;
            _periodoRepasseRepository = periodoRepasseRepository;
            _connectionStrings = connectionStrings;
            _calendarioService = calendarioService;
        }

        public PeriodoRepasse BuscarPorId(int id)
        {
            return _periodoRepasseRepository.BuscarPorId(id);
        }

        public void Persistir(PeriodoRepasse periodoRepasse)
        {
            var periodoDto = CriarEntidadePeriodoParaBroadcast(periodoRepasse, periodoRepasse.Id == 0 ? false : true);

            if (periodoRepasse.Id == 0)
            {
                _periodoRepasseRepository.Adicionar(periodoRepasse);
                EnviarPeriodoParaEacesso(periodoRepasse);
                EnviarEmailRepasse(periodoRepasse);
            }
            else
            {
                _periodoRepasseRepository.Update(periodoRepasse);
                EnviarEmailAtualizacaoRepasse(periodoRepasse);
            }

            if (_unitOfWork.Commit()) { CadastrarBroadcastsParaAberturaDePeriodo(periodoDto); }
        }

        private void EnviarPeriodoParaEacesso(PeriodoRepasse periodoRepasse)
        {
            var celulas = ObterTodasCelulas();

            var connectionString = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                string sQueryInsercao = "";

                foreach (var celula in celulas)
                {
                    string QueryVerificacao = $@"SELECT * FROM tblAbertura_Fechamento_Repasse WHERE ID_CELULA ={celula}  AND DT_ABERTURA_REPASSE ='{periodoRepasse.DtLancamento}' ";
                    var result = dbConnection.Query(QueryVerificacao);
                    if(result.Count() == 0)
                    {
                        sQueryInsercao = $@"insert into stfcorp.tblabertura_fechamento_repasse (ID_CELULA, DT_ABERTURA_REPASSE) values ({celula}, '{periodoRepasse.DtLancamento}')";
                        dbConnection.Execute(sQueryInsercao);
                    }
                }

                dbConnection.Close();
            }
        }

        private List<int> ObterTodasCelulas()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_microServicosUrls.UrlApiControle)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync("api/Celula/eacesso").Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new List<int>();
            }
            else
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<List<Celula>>(jsonString);
                return result.Select(x => x.Id).ToList();
            }
        }

        public FiltroGenericoDtoBase<PeriodoRepasseDto> FiltrarPeriodo(FiltroGenericoDtoBase<PeriodoRepasseDto> filtro)
        {
            var result = _periodoRepasseRepository.FiltrarPeriodo(filtro);
            return result;
        }

        private void EnviarEmailRepasse(PeriodoRepasse periodoRepasse)
        {
            List<ValorParametroEmailDTO> parametros = DefinirParametrosRepasse(periodoRepasse);
            var email = new EmailDto
            {
                IdTemplate = 7,
                RemetenteNome = "Gestão de Repasses",
                Para = ObterDestinatarioEmailPeriodoRepasse(),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
        }

        private void EnviarEmailAtualizacaoRepasse(PeriodoRepasse periodoRepasse)
        {
            List<ValorParametroEmailDTO> parametros = DefinirParametrosRepasse(periodoRepasse);
            var email = new EmailDto
            {
                IdTemplate = 8,
                RemetenteNome = "Gestão de Repasses",
                Para = ObterDestinatarioEmailPeriodoRepasse(),
                ValoresParametro = parametros
            };

            EnviarEmail(email);
        }

        private string ObterDestinatarioEmailPeriodoRepasse()
        {
            string[] funcionalidades = new string[] { "ConsultarPeriodoRepasse",
                                                      "AdicionarPeriodoRepasse",
                                                      "EditarPeriodoRepasse",
                                                      "SolicitarRepasse",
                                                      "EditarRepasse",
                                                      "ConsultarRepasse",
                                                      "AprovarRepasseNivelUm",
                                                      "AprovarRepasseNivelDois" };
            List<string> destinatario = new List<string>();
            switch (Variables.EnvironmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    destinatario.Add("metavares1@latam.Stefanini.com,lpguimaraes@latam.Stefanini.com,masilva21@latam.Stefanini.com,afranco@latam.stefanini.com, clbatista@latam.stefanini.com, rogomes@stefanini.com");
                    break;
                case "STAGING":
                    destinatario = DefinirEmailsDestinatarios(funcionalidades);
                    destinatario.Add("metavares1@latam.Stefanini.com,lpguimaraes@latam.Stefanini.com,masilva21@latam.Stefanini.com,afranco@stefanini.com, clbatista@stefanini.com, rogomes@stefanini.com");
                    break;
                case "QA":
                    destinatario.Add("metavares1@latam.Stefanini.com,lpguimaraes@latam.Stefanini.com,masilva21@latam.Stefanini.com,afranco@stefanini.com, clbatista@stefanini.com, rogomes@stefanini.com");
                    break;
                case "PREPRODUCTION":
                    destinatario.Add("metavares1@latam.Stefanini.com,lpguimaraes@latam.Stefanini.com,masilva21@latam.Stefanini.com,afranco@stefanini.com, clbatista@stefanini.com, rogomes@stefanini.com");
                    
                    break;
                case "PRODUCTION":
                    destinatario = DefinirEmailsDestinatarios(funcionalidades);
                    break;
            }
            return string.Join(",", destinatario);
        }

        private void EnviarEmail(EmailDto email)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlEnvioEmail);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            try {

                var response = client.PostAsync("api/Email", content).Result;

                var responseString = response.Content.ReadAsStringAsync().Result;
            }
            catch(Exception e)
            {

            }
        }

        private List<string> DefinirEmailsDestinatarios(string[] funcionalidades)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(funcionalidades), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/usuarioperfil/obter-email-por-funcionalidades", content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<string>>(responseString);
        }

        private List<ValorParametroEmailDTO> DefinirParametrosRepasse(PeriodoRepasse periodoRepasse)
        {
            var parametros = new List<ValorParametroEmailDTO>();
            var parametroMesAnoRepasseLancamento = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroMesAnoRepasseLancamento]",
                ParametroValor = periodoRepasse.DtLancamento.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroMesAnoRepasseLancamento);

            var parametroDataInicioLancamento = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataInicioLancamento]",
                ParametroValor = periodoRepasse.DtLancamentoInicio.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDataInicioLancamento);

            var parametroDataFimLancamento = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataFimLancamento]",
                ParametroValor = periodoRepasse.DtLancamentoFim.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDataFimLancamento);

            var parametroDataInicioAprovacao1Nivel = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataInicioAprovacao1Nivel]",
                ParametroValor = periodoRepasse.DtAnaliseInicio.ToString("dd/MM/yyyy")
            };

            parametros.Add(parametroDataInicioAprovacao1Nivel);

            var parametroDataFimAprovacao1Nivel = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataFimAprovacao1Nivel]",
                ParametroValor = periodoRepasse.DtAnaliseFim.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDataFimAprovacao1Nivel);

            var parametroDataInicioAprovacao2Nivel = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataInicioAprovacao2Nivel]",
                ParametroValor = periodoRepasse.DtAprovacaoInicio.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDataInicioAprovacao2Nivel);

            var parametroDataFimAprovacao2Nivel = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroDataFimAprovacao2Nivel]",
                ParametroValor = periodoRepasse.DtAprovacaoFim.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDataFimAprovacao2Nivel);

            var parametroMesAnoRepasseReferencia = new ValorParametroEmailDTO
            {
                ParametroNome = "[ParametroMesAnoRepasseReferencia]",
                ParametroValor = periodoRepasse.DtLancamento.AddMonths(-1).ToString("dd/MM/yyyy")

            };
            parametros.Add(parametroMesAnoRepasseReferencia);

            var parametroDatatFechamento = new ValorParametroEmailDTO
            {
                ParametroNome = " [ParametroDataFechamento]",
                ParametroValor = periodoRepasse.DtAprovacaoFim.ToString("dd/MM/yyyy")
            };
            parametros.Add(parametroDatatFechamento);
            return parametros;
        }

        public DateTime ObterDataDiasUteis(DateTime data, int qtdDias)
        {

            var listaFeriados = _calendarioService.ObterFeriadosNacionais(data.Year)
                .Where(d => d.Type.ToUpper() == "FERIADO NACIONAL");

            var diasUteis = 0;
            var diaAux = data.AddDays(0);

            while (diasUteis < qtdDias)
            {
               
                if (diaAux.DayOfWeek != DayOfWeek.Saturday && diaAux.DayOfWeek != DayOfWeek.Sunday &&
                    !listaFeriados.Any(d => d.Date == diaAux))
                {
                    if (diasUteis == 0)
                    {
                        diaAux = diaAux.AddDays(1);

                    }
                    diasUteis++;
                }

                diaAux = diaAux.AddDays(1);
            }
            while (diasUteis > qtdDias)
            {
                if (diaAux.DayOfWeek != DayOfWeek.Saturday && diaAux.DayOfWeek != DayOfWeek.Sunday &&
                    !listaFeriados.Any(d => d.Date == diaAux))
                {
                    diasUteis--;
                }
                diaAux = diaAux.AddDays(-1);

            }

            return diaAux;
        }

        public PeriodoRepasse BuscarPeriodoVigente()
        {
            var periodoRepasse = _periodoRepasseRepository.BuscarPeriodoVigente();
            return periodoRepasse;
        }

        public void PopularDataFimLancamentoEacesso(PeriodoRepasse periodoVigente)
        {
            var connectionString = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                string sQueryInsercao = "";

                sQueryInsercao = $@"UPDATE STFCORP.TBLABERTURA_FECHAMENTO_REPASSE SET DT_FECHAMENTO_LANCTO = '{periodoVigente.DtLancamentoFim}' 
                                    WHERE DT_ABERTURA_REPASSE = '{periodoVigente.DtLancamento}'";
                dbConnection.Execute(sQueryInsercao);

                dbConnection.Close();
            }
        }

        public void PopularDataFimEacesso(PeriodoRepasse periodoVigente)
        {
            var connectionString = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                string sQueryInsercao = "";

                sQueryInsercao = $@"UPDATE STFCORP.TBLABERTURA_FECHAMENTO_REPASSE SET DT_FECHAMENTO_REPASSE = '{periodoVigente.DtAprovacaoFim}' 
                                    WHERE DT_ABERTURA_REPASSE = '{periodoVigente.DtLancamento}'";
                dbConnection.Execute(sQueryInsercao);

                dbConnection.Close();
            }
        }

        private bool CadastrarBroadcastsParaAberturaDePeriodo(PeriodoRepasseDto periodo)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(_microServicosUrls.UrlApiControle);
                var requestBody = new StringContent(JsonConvert.SerializeObject(periodo), Encoding.UTF8, "application/json");
                
                var res = http.PutAsync("api/broadcast/criar-broadcasts-abertura-periodo-repasse", requestBody).Result;
                var json = res.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<StfCorpHttpListResult<BroadcastDto>>(json);
                return result.Success;
            }
        }

        private PeriodoRepasseDto CriarEntidadePeriodoParaBroadcast(PeriodoRepasse periodo, bool ehAlteracaoCronograma)
        {
            return new PeriodoRepasseDto
            {
                DtLancamentoInicio = periodo.DtLancamentoInicio,
                DtLancamentoFim = periodo.DtLancamentoFim,
                DtAnaliseInicio = periodo.DtAnaliseInicio,
                DtAnaliseFim = periodo.DtAnaliseFim,
                DtAprovacaoInicio = periodo.DtAprovacaoInicio,
                DtAprovacaoFim = periodo.DtAprovacaoFim,
                DtLancamento = periodo.DtLancamento,
                EhAlteracaoCronograma = ehAlteracaoCronograma
            };
        }
    }
}