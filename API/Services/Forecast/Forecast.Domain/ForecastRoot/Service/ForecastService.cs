using Dapper;
using Forecast.Api.ViewModels;
using Forecast.Domain.ForecastRoot.Attributes;
using Forecast.Domain.ForecastRoot.Dto;
using Forecast.Domain.ForecastRoot.Repository;
using Forecast.Domain.ForecastRoot.Service.Interfaces;
using Forecast.Domain.SharedRoot;
using Logger.Context;
using Logger.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils;
using Utils.Base;
using Utils.Calendario;
using Utils.Calendario.Model;
using Utils.Connections;

namespace Forecast.Domain.ForecastRoot.Service
{
    public class ForecastService : IForecastService
    {
        private readonly IForecastRepository _forecastRepository;
        private readonly IValorForecastRepository _valorForecastRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly ICalendarioService _calendarioService;
        private readonly IConfiguration _configuration;
        private readonly MicroServicosUrls _microServicosUrls;

        public ForecastService(IForecastRepository forecastRepository,
                               IValorForecastRepository valorForecastRepository,
                               IOptions<ConnectionStrings> connectionStrings,
                               ICalendarioService calendarioService,
                               IConfiguration configuration,                               
                               IUnitOfWork unitOfWork, 
                               MicroServicosUrls microServicosUrls)
        {
            _forecastRepository = forecastRepository;
            _valorForecastRepository = valorForecastRepository;
            _unitOfWork = unitOfWork;
            _microServicosUrls = microServicosUrls;
            _connectionStrings = connectionStrings;
            _calendarioService = calendarioService;
            _configuration = configuration;
        }

        public void Adicionar(ForecastET forecast)
        {
            var forecastExiste = _forecastRepository
                .Buscar(forecast.IdCelula, forecast.IdCliente, forecast.IdServico, forecast.NrAno);
            ForecastET forecastAnoSeguinteExiste = null;
            if (forecast.DataAniversario != null)
            {
                forecastAnoSeguinteExiste = forecast.DataAniversario.Value.Year > forecast.NrAno ?
                _forecastRepository.Buscar(forecast.IdCelula, forecast.IdCliente, forecast.IdServico,
                forecast.DataAniversario.Value.Year) :
                null;
            }

            if (forecastExiste != null || forecastAnoSeguinteExiste != null)
                throw new ArgumentException($"Um Forecast para esta Célula, Cliente, Serviço e Ano já existe");

            _forecastRepository.Adicionar(forecast);
            if (forecast.DataAniversario.HasValue && forecast.DataAniversario.Value.Year > forecast.NrAno)
            {
                var forecastDoAnoSeguinte = CriarEntidadeForecastDoAnoSeguinte(forecast, AtribuirValoresDosMesesParaAnoSeguinteAdicao);
                var forecastDoSegundoAnoSeguinte = CriarEntidadeForecastDoAnoSeguinte(forecastDoAnoSeguinte, AtribuirValoresDosMesesParaAnoSeguinteAdicao);
                _forecastRepository.Adicionar(forecastDoAnoSeguinte);
                _forecastRepository.Adicionar(forecastDoSegundoAnoSeguinte);
            }

            _unitOfWork.Commit();
        }

        public void Atualizar(ForecastET forecast)
        {
            _forecastRepository.Update(forecast);
            var forecastAnoSeguinte = _forecastRepository
                .BuscarPorIdComIncludes(forecast.IdCelula, forecast.IdCliente, forecast.IdServico, forecast.NrAno + 1);

            if (forecast.DataAniversario != null && forecast.DataAniversario.Value.Year > forecast.NrAno)
            {
                if (forecastAnoSeguinte == null)
                {
                    forecastAnoSeguinte = CriarEntidadeForecastDoAnoSeguinte
                        (forecast, AtribuirValoresDosMesesParaAnoSeguinteEdicao);
                    _forecastRepository.Adicionar(forecastAnoSeguinte);
                }
                else
                {
                    AtualizarEntidadeForecastAnoSeguinte(forecast, forecastAnoSeguinte);
                    _forecastRepository.Update(forecastAnoSeguinte);
                }

                var forecastSegundoAnoSeguinte = _forecastRepository
                    .BuscarPorIdComIncludes(forecastAnoSeguinte.IdCelula, forecastAnoSeguinte.IdCliente,
                    forecastAnoSeguinte.IdServico, forecastAnoSeguinte.NrAno + 1);
                if (forecastSegundoAnoSeguinte == null)
                {
                    forecastSegundoAnoSeguinte = CriarEntidadeForecastDoAnoSeguinte
                        (forecastAnoSeguinte, AtribuirValoresDosMesesParaAnoSeguinteEdicao);
                    _forecastRepository.Adicionar(forecastSegundoAnoSeguinte);
                }
                else
                {
                    AtualizarEntidadeForecastAnoSeguinte(forecastAnoSeguinte, forecastSegundoAnoSeguinte);
                    _forecastRepository.Update(forecastSegundoAnoSeguinte);
                }
            }

            //if (forecast != null &&  forecast.ValorForecast != null && forecast.ValorForecast.VlPercentual != null)
            //{
            //    forecast.ValorForecast.VlPercentual = forecast.ValorForecast.VlPercentual * 100;
            //}

            //if (forecastAnoSeguinte != null && forecastAnoSeguinte.ValorForecast != null && forecastAnoSeguinte.ValorForecast.VlPercentual != null)
            //{
            //    forecastAnoSeguinte.ValorForecast.VlPercentual = forecastAnoSeguinte.ValorForecast.VlPercentual * 100;
            //}

            _unitOfWork.Commit();
        }

        public List<ForecastET> BuscarTodos()
        {
            var forecasts = _forecastRepository.BuscarTodos().ToList();
            return forecasts;
        }

        public List<int> BuscarTodosAnos()
        {
            var forecasts = _forecastRepository.BuscarTodos().OrderByDescending(x => x.NrAno).ToList();
            var anoLista = forecasts.Select(f => f.NrAno).Distinct().ToList();
            return anoLista;
        }

        public ForecastET BuscarPorId(int id)
        {
            var result = _forecastRepository.BuscarPorId(id);
            return result;
        }

        public ForecastET BuscarPorIdComposto(int idCelula, int idCliente, int idServico, int ano)
        {
            var result = _forecastRepository.Buscar(idCelula, idCliente, idServico, ano);
            return result;
        }

        public FiltroGenericoDtoBase<ForecastDto> Filtrar(FiltroGenericoDtoBase<ForecastDto> filtro)
        {
            List<int> idClientes = null;
            List<int> idServicos = null;
            if (!String.IsNullOrEmpty(filtro.FiltroGenerico))
            {
                idClientes = BuscarIdClientesPorNomeCliente(filtro.FiltroGenerico);
                idServicos = BuscarIdServicosPorNomeServico(filtro.FiltroGenerico);
            }

            var result = _forecastRepository.Filtrar(filtro, idClientes ?? null, idServicos ?? null);

            var listaClientes = result.Valores.Select(f => f.IdCliente).Distinct().ToList();
            var clientes = String.Join(",", listaClientes);


            if (!String.IsNullOrEmpty(clientes))
            {

                var listaClientesDto = ObterClientePorIdsEAcesso(clientes);

                result.Valores.ForEach(f =>
                {
                    var cliente = listaClientesDto.ToList().Find(c => c.Id == f.IdCliente);
                    if (cliente != null)
                    {
                        f.NomeCliente = cliente.Descricao;
                    }
                });
            }

            var listaServicos = result.Valores.Select(f => f.IdServico).Distinct().ToList();
            var servicos = String.Join(",", listaServicos);

            if (!String.IsNullOrEmpty(servicos))
            {
                var listaServicosDto = ObterServicosPorIdsEacesso(servicos);

                result.Valores.ForEach(f =>
                {
                    var servico = listaServicosDto.ToList().Find(c => c.Id == f.IdServico);
                    if (servico != null)
                    {
                        f.NomeServico = servico.Descricao;
                    }
                });
            }
            if (!string.IsNullOrEmpty(filtro.FiltroGenerico) && filtro.FiltroGenerico.Any())
            {
                result.Valores = result.Valores.Where(x =>
                                       x.IdCelula.ToString().ToUpper().Equals(filtro.FiltroGenerico.ToUpper())
                                       || (x.NomeCliente != null ? x.NomeCliente.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()) : false)
                                       || (x.NomeServico != null ? x.NomeServico.ToUpper().Contains(filtro.FiltroGenerico.ToUpper()) : false)
                                    ).ToList();
            }

            if (filtro.OrdemOrdenacao == "asc")
            {
                result.Valores = result.Valores.OrderBy(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else if (filtro.OrdemOrdenacao == "desc")
            {
                result.Valores = result.Valores.OrderByDescending(x => x.GetType().GetProperty(filtro.CampoOrdenacao).GetValue(x)).ToList();
            }
            else
            {
                result.Valores = result.Valores.OrderBy(x => x.IdCelula).ThenBy(x => x.NomeServico).ThenBy(y => y.NomeCliente).ToList();
            }

            filtro.Valores = result.Valores.Skip((filtro.Pagina) * filtro.QuantidadePorPagina).Take(filtro.QuantidadePorPagina).ToList();



            filtro.Valores = DefinirAlertaAniversarioDeForecasts(filtro.Valores);


            return result;
        }

        private List<ForecastDto> DefinirAlertaAniversarioDeForecasts(List<ForecastDto> valores)
        {
            foreach (var forecast in valores)
            {
                forecast.AlertaDataAniversario = _forecastRepository.DefinirAlertaAniversarioForecast(forecast);
            }
            return valores;
        }


        public List<ComboDefaultDto> ObterClientePorIdsEAcesso(string clientes)
        {
            List<ComboDefaultDto> clientesDto = new List<ComboDefaultDto>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select DISTINCT c.idCliente as id, c.nomeFantasia as descricao from stfcorp.tblclientes c, stfcorp.tblClientesServicos s where c.idCliente = s.idCliente and c.idCliente in (" + clientes + ") ORDER BY descricao;";
                clientesDto = dbConnection.Query<ComboDefaultDto>(query).ToList();
                dbConnection.Close();
            }
            return clientesDto;
        }


        private List<int> BuscarIdClientesPorNomeCliente(string nome)
        {
            List<int> idClientes = new List<int>();
            using (IDbConnection dbConnection = new SqlConnection(_connectionStrings.Value.EacessoConnection))
            {
                var query = $"SELECT c.idCliente as id FROM stfcorp.tblClientes c WHERE c.nomeFantasia LIKE '%{nome}%';";
                idClientes = dbConnection.Query<int>(query).ToList();
            }
            return idClientes;
        }

        private List<int> BuscarIdServicosPorNomeServico(string nome)
        {
            var client = new HttpClient { BaseAddress = new Uri(_microServicosUrls.UrlApiServico) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync($"api/ServicoContratados/filtrar-servico-por-nome/{nome}").Result;

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<int>>(jsonString);
            return result;
        }

        public List<ComboDefaultDto> ObterServicosPorIdsEacesso(string servicos)
        {
            var client = new HttpClient { BaseAddress = new Uri(_microServicosUrls.UrlApiServico) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync($"api/ServicoContratados/obter-servicos-por-ids/{servicos}").Result;

            var jsonString = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<ComboDefaultDto>>(jsonString);
            return result;
        }

        private ForecastET CriarEntidadeForecastDoAnoSeguinte(ForecastET forecast, Action<ForecastET, ForecastET> funcAtribuicaoValorMeses)
        {
            ForecastET forecastDoAnoSeguinte = (ForecastET)forecast.Clone();
            forecastDoAnoSeguinte.ValorForecast = (ValorForecast)forecast.ValorForecast.Clone();
            forecastDoAnoSeguinte.NrAno++;
            forecastDoAnoSeguinte.ValorForecast.NrAno++;
            //AtribuirValoresDosMesesParaAnoSeguinte(forecast, forecastDoAnoSeguinte);
            funcAtribuicaoValorMeses(forecast, forecastDoAnoSeguinte);
            return forecastDoAnoSeguinte;
        }

        private void AtualizarEntidadeForecastAnoSeguinte(ForecastET forecast, ForecastET forecastAnoSeguinte)
        {
            forecastAnoSeguinte.DescricaoJustificativa = forecast.DescricaoJustificativa;
            forecastAnoSeguinte.IdStatus = forecast.IdStatus;
            forecastAnoSeguinte.DataAniversario = forecast.DataAniversario;
            forecastAnoSeguinte.DataAplicacaoReajuste = forecast.DataAplicacaoReajuste;
            forecastAnoSeguinte.DataReajusteRetroativo = forecast.DataReajusteRetroativo;
            forecastAnoSeguinte.ValorForecast.VlPercentual = forecast.ValorForecast.VlPercentual;
            forecastAnoSeguinte.ValorForecast.ValorAjuste = forecast.ValorForecast.ValorAjuste;
            AtribuirValoresDosMesesParaAnoSeguinteEdicao(forecast, forecastAnoSeguinte);
        }

        private void AtribuirValoresDosMesesParaAnoSeguinteEdicao(ForecastET forecast, ForecastET forecastAnoSeguinte = null)
        {
            var mesesPropertiesList = forecast.ValorForecast.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(MonthNumber)));

            decimal? valorAux = 0;

            //if (forecast.DataAniversario.Value.Year > forecast.NrAno)
            //{
            foreach (var mesProperty in mesesPropertiesList)
            {
                if ((decimal?)mesProperty.GetValue(forecast.ValorForecast) != 0)
                    valorAux = (decimal?)mesProperty.GetValue(forecast.ValorForecast);
            }
            //}
            //else
            //{
            //    var valorAniversario = (decimal?)forecast.ValorForecast.GetType().GetProperties()
            //        .Where(x => x.CustomAttributes.Any(y => y.ConstructorArguments.Any(z => (int)z.Value == forecast.DataAniversario.Value.Month)))
            //        .FirstOrDefault().GetValue(forecast.ValorForecast);
            //    valorAux = valorAniversario;
            //}

            foreach (var mesProperty in mesesPropertiesList)
            {
                var numeroDoMes = (int)(mesProperty.CustomAttributes.FirstOrDefault().ConstructorArguments
                    .FirstOrDefault().Value);
                //if (numeroDoMes >= forecast.DataReajusteRetroativo.Value.Month && numeroDoMes <= forecast?.DataAniversario.Value.Month)
                //mesProperty.SetValue(forecastAnoSeguinte.ValorForecast, valorAux + 
                //    (valorAux * (forecastAnoSeguinte.ValorForecast.VlPercentual)));
                mesProperty.SetValue(forecastAnoSeguinte.ValorForecast, valorAux);
                //else
                //    mesProperty.SetValue(forecastAnoSeguinte.ValorForecast, (decimal?)0);
            }
        }

        private void AtribuirValoresDosMesesParaAnoSeguinteAdicao(ForecastET forecast, ForecastET forecastAnoSeguinte = null)
        {
            var mesesPropertiesList = forecast.ValorForecast.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(MonthNumber)));
            decimal? valorDoUltimoMesPreenchdo = 0;
            foreach (var mesProperty in mesesPropertiesList)
            {
                if ((decimal?)mesProperty.GetValue(forecast.ValorForecast) != 0)
                    valorDoUltimoMesPreenchdo = (decimal?)mesProperty.GetValue(forecast.ValorForecast);
            }

            foreach (var mesProperty in mesesPropertiesList)
            {
                var numeroDoMes = (int)(mesProperty.CustomAttributes.FirstOrDefault().ConstructorArguments.FirstOrDefault().Value);
                //if (numeroDoMes <= forecast.DataAniversario.Value.Month)
                mesProperty.SetValue(forecastAnoSeguinte.ValorForecast, valorDoUltimoMesPreenchdo);
                //else
                //    mesProperty.SetValue(forecastAnoSeguinte.ValorForecast, (decimal?)0);
            }
        }

        public ForecastET VerificarSeRegistroExiste(ForecastET forecast)
        {
            return _forecastRepository.Buscar(forecast.IdCelula, forecast.IdCliente, forecast.IdServico, forecast.NrAno);
        }


        public void RealizarMigracao()
        {
            List<ForecastET> forecasts = new List<ForecastET>();

            List<ForecastET> forecastsPorAno = new List<ForecastET>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();

                var listaAno = ObterEfaturamentoAnos(dbConnection);

                listaAno.ForEach(ano =>
                {
                    var efaturamentoAgrupadoDtoLista = ObterEfaturamentoAgrupadoDto(dbConnection, ano).ToList();

                    foreach (var efaturamentoAgrupadoDto in efaturamentoAgrupadoDtoLista)
                    {
                        List<EfaturamentoAgrupadoDto> efaturamentoAgrupadoDtoGrupoLista = new List<EfaturamentoAgrupadoDto>();
                        try
                        {
                            var efaturamentoAgrupadoDtoGrupoExiste = forecastsPorAno.Any(e =>

                                    e.IdCelula == efaturamentoAgrupadoDto.IdCelula &&
                                    e.IdCliente == efaturamentoAgrupadoDto.IdCliente &&
                                    e.IdServico == efaturamentoAgrupadoDto.IdServico &&
                                    e.NrAno == ano
                              );

                            if (!efaturamentoAgrupadoDtoGrupoExiste)
                            {

                                efaturamentoAgrupadoDtoGrupoLista = efaturamentoAgrupadoDtoLista.FindAll(e =>

                                       e.IdCelula == efaturamentoAgrupadoDto.IdCelula &&
                                       e.IdCliente == efaturamentoAgrupadoDto.IdCliente &&
                                       e.IdServico == efaturamentoAgrupadoDto.IdServico &&
                                       e.Ano == ano
                                 );

                                if (efaturamentoAgrupadoDtoGrupoLista != null && efaturamentoAgrupadoDtoGrupoLista.Count > 0)
                                {
                                    var efaturamentoAgrupadoDtoGrupo = new EfaturamentoAgrupadoDto
                                    {
                                        IdCelula = efaturamentoAgrupadoDtoGrupoLista.First().IdCelula,
                                        IdCliente = efaturamentoAgrupadoDtoGrupoLista.First().IdCliente,
                                        IdServico = efaturamentoAgrupadoDtoGrupoLista.First().IdServico,
                                        Ano = efaturamentoAgrupadoDtoGrupoLista.First().Ano,
                                        ValorRecorrente = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrente),
                                        ValorRecorrenteNao = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrenteNao),
                                        ValorRecorrenteNovasVendas = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrenteNovasVendas),
                                        ValorRecorrentePerdas = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrentePerdas),
                                        ValorRecorrenteMultas = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrenteMultas),
                                        ValorRecorrenteRepactuacao = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrenteRepactuacao),
                                        ValorRecorrenteRepactuacaoRetro = efaturamentoAgrupadoDtoGrupoLista.Sum(x => x.ValorRecorrenteRepactuacaoRetro),
                                    };

                                    var forecastET = new ForecastET
                                    {
                                        IdCelula = efaturamentoAgrupadoDtoGrupo.IdCelula,
                                        IdCliente = efaturamentoAgrupadoDtoGrupo.IdCliente,
                                        IdServico = efaturamentoAgrupadoDtoGrupo.IdServico,
                                        NrAno = efaturamentoAgrupadoDtoGrupo.Ano,
                                        DataAplicacaoReajuste = ObterDataAplicacaoReajuste(dbConnection, efaturamentoAgrupadoDtoGrupo.IdCelula, efaturamentoAgrupadoDtoGrupo.IdCliente, efaturamentoAgrupadoDtoGrupo.IdServico, ano),
                                        DataReajusteRetroativo = ObterDataReajusteRetroativo(dbConnection, efaturamentoAgrupadoDtoGrupo.IdCelula, efaturamentoAgrupadoDtoGrupo.IdCliente, efaturamentoAgrupadoDtoGrupo.IdServico, ano),
                                        //IdStatus = 0, //TODO EM BRANCO
                                        Usuario = "Eacesso",
                                        DataAlteracao = DateTime.Now,
                                        //DescricaoJustificativa = "Teste de migração",
                                        ValorForecast = new ValorForecast
                                        {
                                            VlPercentual = ObterValorPercentual(efaturamentoAgrupadoDtoGrupo),
                                            ValorJaneiro = ObterValoreMes(ano, 1, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorFevereiro = ObterValoreMes(ano, 2, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorMarco = ObterValoreMes(ano, 3, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorAbril = ObterValoreMes(ano, 4, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorMaio = ObterValoreMes(ano, 5, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorJunho = ObterValoreMes(ano, 6, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorJulho = ObterValoreMes(ano, 7, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorAgosto = ObterValoreMes(ano, 8, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorSetembro = ObterValoreMes(ano, 9, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorOutubro = ObterValoreMes(ano, 10, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorNovembro = ObterValoreMes(ano, 11, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            ValorDezembro = ObterValoreMes(ano, 12, efaturamentoAgrupadoDtoLista, efaturamentoAgrupadoDto),
                                            Usuario = "Eacesso",
                                            DataAlteracao = DateTime.Now
                                        }
                                    };
                                    adicionarValorTotal(forecastET);
                                    //TODO Data de Aniversário para vir em branco
                                    //forecastET.DataAniversario = ObterDataAniversario(forecastET);

                                    forecastsPorAno.Add(forecastET);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            var erroMsg = "IdCelula:" + efaturamentoAgrupadoDtoGrupoLista.First().IdCelula.ToString() + " IdCliente: " + efaturamentoAgrupadoDtoGrupoLista.First().IdCliente.ToString() + " IdServico:" + efaturamentoAgrupadoDtoGrupoLista.First().IdServico.ToString();
                            AdicionarLogGenerico(ex, "Erro ao migrar dados FORECAST ano:" + ano.ToString(), erroMsg);
                            continue;
                        }
                    }

                    _forecastRepository.AdicionarRange(forecastsPorAno);
                    _unitOfWork.Commit();

                    forecastsPorAno = new List<ForecastET>();
                });
                dbConnection.Close();
            }
        }

        private void adicionarValorTotal(ForecastET forecastET)
        {
            forecastET.ValorForecast.ValorTotal = forecastET.ValorForecast.ValorJaneiro + forecastET.ValorForecast.ValorFevereiro + forecastET.ValorForecast.ValorMarco
                                    + forecastET.ValorForecast.ValorAbril + forecastET.ValorForecast.ValorMaio + forecastET.ValorForecast.ValorJunho
                                    + forecastET.ValorForecast.ValorJulho + forecastET.ValorForecast.ValorAgosto + forecastET.ValorForecast.ValorSetembro
                                    + forecastET.ValorForecast.ValorOutubro + forecastET.ValorForecast.ValorNovembro + forecastET.ValorForecast.ValorDezembro;
        }

        private decimal? ObterValorPercentual(EfaturamentoAgrupadoDto efaturamentoAgrupadoDto)
        {
            if (efaturamentoAgrupadoDto.ValorRecorrenteRepactuacao > 0 &&
                efaturamentoAgrupadoDto.ValorRecorrente > 0)
            {
                var valorDividir = efaturamentoAgrupadoDto.ValorRecorrente
                       + efaturamentoAgrupadoDto.ValorRecorrenteNao
                       + efaturamentoAgrupadoDto.ValorRecorrenteNovasVendas
                       - efaturamentoAgrupadoDto.ValorRecorrentePerdas;
                if (valorDividir > 0)
                {
                    var valorPercentual = (efaturamentoAgrupadoDto.ValorRecorrenteRepactuacao / valorDividir) * 100;
                    return valorPercentual;
                }
            }

            return null;
        }

        private DateTime? ObterDataAniversario(ForecastET forecastET)
        {
            DateTime? dataAniversario = null;

            if (forecastET.NrAno > 0)
            {
                var ano = forecastET.NrAno + 1;

                if (forecastET.ValorForecast.ValorJaneiro > 0)
                {
                    dataAniversario = new DateTime(ano, 1, 1);
                }

                if (forecastET.ValorForecast.ValorFevereiro > 0)
                {
                    dataAniversario = new DateTime(ano, 2, 1);
                }

                if (forecastET.ValorForecast.ValorMarco > 0)
                {
                    dataAniversario = new DateTime(ano, 3, 1);
                }
                if (forecastET.ValorForecast.ValorAbril > 0)
                {
                    dataAniversario = new DateTime(ano, 4, 1);
                }
                if (forecastET.ValorForecast.ValorMaio > 0)
                {
                    dataAniversario = new DateTime(ano, 5, 1);
                }
                if (forecastET.ValorForecast.ValorJunho > 0)
                {
                    dataAniversario = new DateTime(ano, 6, 1);
                }
                if (forecastET.ValorForecast.ValorJulho > 0)
                {
                    dataAniversario = new DateTime(ano, 7, 1);
                }
                if (forecastET.ValorForecast.ValorAgosto > 0)
                {
                    dataAniversario = new DateTime(ano, 8, 1);
                }
                if (forecastET.ValorForecast.ValorSetembro > 0)
                {
                    dataAniversario = new DateTime(ano, 9, 1);
                }
                if (forecastET.ValorForecast.ValorOutubro > 0)
                {
                    dataAniversario = new DateTime(ano, 10, 1);
                }
                if (forecastET.ValorForecast.ValorNovembro > 0)
                {
                    dataAniversario = new DateTime(ano, 11, 1);
                }
                if (forecastET.ValorForecast.ValorDezembro > 0)
                {
                    dataAniversario = new DateTime(ano, 12, 1);
                }
            }

            return dataAniversario;
        }

        private Decimal ObterValoreMes(int? ano,
                                  int? mes,
                                  List<EfaturamentoAgrupadoDto> efaturamentoAgrupadoDtoLista,
                                  EfaturamentoAgrupadoDto efaturamentoAgrupadoDto)
        {
            decimal valorMes = 0;
            var efaturamentoAgrupadoDtoMes = efaturamentoAgrupadoDtoLista.Find(e =>

                                                e.IdCelula == efaturamentoAgrupadoDto.IdCelula &&
                                                e.IdCliente == efaturamentoAgrupadoDto.IdCliente &&
                                                e.IdServico == efaturamentoAgrupadoDto.IdServico &&
                                                e.Ano == ano &&
                                                e.Mes == mes
                                          );
            if (efaturamentoAgrupadoDtoMes != null)
            {
                valorMes = (efaturamentoAgrupadoDtoMes.ValorRecorrente
                       + efaturamentoAgrupadoDtoMes.ValorRecorrenteNao
                       + efaturamentoAgrupadoDtoMes.ValorRecorrenteNovasVendas
                       - efaturamentoAgrupadoDtoMes.ValorRecorrentePerdas
                       - efaturamentoAgrupadoDtoMes.ValorRecorrenteMultas
                       //+ efaturamentoAgrupadoDtoMes.ValorRecorrenteRepactuacao Não faz Parte
                       //+ efaturamentoAgrupadoDtoMes.ValorRecorrenteRepactuacaoRetro
                       );
            }
            return valorMes;
        }

        public List<int> ObterEfaturamentoAnos(IDbConnection dbConnection)
        {
            string sQuery = @"SELECT DISTINCT CONVERT(VARCHAR, YEAR(R.DATA))	AS 'ANO'
                              FROM [stfcorp].EFATURAMENTO_REVENUEINFORCE R 			
                              LEFT JOIN	[stfcorp].tblClientes CLI ON CLI.IDCLIENTE = R.IDCLIENTE 
                              LEFT JOIN	[stfcorp].TBLCLIENTESSERVICOS SRV ON SRV.IDSERVICO = R.IDSERVICO 
                              WHERE CONVERT(VARCHAR, YEAR(R.DATA)) >= 2017
                              ORDER BY CONVERT(VARCHAR, YEAR(R.DATA)) ";
            var result = dbConnection.Query<int>(sQuery).AsList();
            return result;
        }

        public List<EfaturamentoAgrupadoDto> ObterEfaturamentoAgrupadoDto(IDbConnection dbConnection, int ano)
        {
            string sQuery = @"SELECT R.IDCELULA	AS 'IdCelula'
			                         , CLI.IDCLIENTE AS 'IdCliente'
			                         , SRV.IDSERVICO AS 'IdServico'
			                         , CONVERT(VARCHAR, YEAR(R.DATA)) AS 'Ano' 
                                     , SUM(VLR_RECORRENTE) AS 'ValorRecorrente'
			                         , SUM(VLR_RECORRENTENAO) AS 'ValorRecorrenteNao'
			                         , SUM(VLR_RECORRENTENOVASVENDAS) AS 'ValorRecorrenteNovasVendas'
			                         , SUM(VLR_RECORRENTEPERDAS) AS 'ValorRecorrentePerdas'
			                         , SUM(VLR_RECORRENTEMULTAS) AS 'ValorRecorrenteMultas'
			                         , SUM(VLR_RECORRENTEREPACTUACAO) AS 'ValorRecorrenteRepactuacao'
			                         , SUM(VLR_RECORRENTEREPACTUACAORETRO)	AS 'ValorRecorrenteRepactuacaoRetro'
			                         , CONVERT(VARCHAR, MONTH(R.DATA)) AS 'Mes'
                            FROM		[stfcorp].EFATURAMENTO_REVENUEINFORCE R 			
                            LEFT JOIN	[stfcorp].tblClientes					CLI ON CLI.IDCLIENTE = R.IDCLIENTE 
                            LEFT JOIN	[stfcorp].TBLCLIENTESSERVICOS			SRV ON SRV.IDSERVICO = R.IDSERVICO 
                            WHERE CONVERT(VARCHAR, YEAR(R.DATA)) = @ano
                              --AND R.IDCELULA = 52 -- TODO Usar para teste
                              --AND R.IdCliente = 1496 -- TODO Usar para teste
                              --AND SRV.IDSERVICO = 39395 -- TODO Usar para teste
                            GROUP BY      R.IDCELULA
			                            , CLI.IDCLIENTE
			                            , SRV.IDSERVICO
			                            , CONVERT(VARCHAR, YEAR(R.DATA))
			                            , CONVERT(VARCHAR, MONTH(R.DATA));";
            var efaturamentoAgrupadoDtoLista = dbConnection.Query<EfaturamentoAgrupadoDto>(sQuery, new { ano }).AsList();
            return efaturamentoAgrupadoDtoLista.OrderByDescending(x => x.IdCelula).OrderByDescending(x => x.IdCliente).OrderByDescending(x => x.IdServico).ToList();
        }

        public DateTime? ObterDataAplicacaoReajuste(IDbConnection dbConnection,
                                                    int idCelula,
                                                    int idCliente,
                                                    int idServico,
                                                    int ano)
        {


            string sQuery = @"SELECT DATA as DataAplicacaoReajuste
	                          FROM [stfcorp].EFATURAMENTO_RevenueInForce 
	                          WHERE VLR_RECORRENTEREPACTUACAO > 0 
	                          AND IDCELULA = @idCelula  
	                          AND IDCLIENTE = @idCliente
	                          AND IDSERVICO = @idServico";
            var result = dbConnection.Query<DateTime?>(sQuery, new { idCelula, idCliente, idServico }).FirstOrDefault();

            if (result != null && result.Value.Year == ano)
            {
                return result;
            }

            return null;
        }

        public DateTime? ObterDataReajusteRetroativo(IDbConnection dbConnection,
                                                    int idCelula,
                                                    int idCliente,
                                                    int idServico,
                                                    int ano)
        {
            string sQuery = @"SELECT DATA as DataReajusteRetroativo
	                          FROM [stfcorp].EFATURAMENTO_RevenueInForce 
	                          WHERE VLR_RECORRENTEREPACTUACAORETRO > 0 
	                          AND IDCELULA = @idCelula  
	                          AND IDCLIENTE = @idCliente
	                          AND IDSERVICO = @idServico";
            var result = dbConnection.Query<DateTime?>(sQuery, new { idCelula, idCliente, idServico }).FirstOrDefault();
            if (result != null && result.Value.Year == ano)
            {
                return result;
            }

            return null;
        }

        private static void AdicionarLogGenerico(Exception ex, string nmOrigem, string descLogGenerico)
        {
            var _logContext = new LogGenericoContext();
            _logContext.LogGenericos.Add(new LogGenerico
            {
                NmTipoLog = "MIGRACAO",
                NmOrigem = nmOrigem,
                DtHoraLogGenerico = DateTime.Now,
                DescLogGenerico = descLogGenerico + " - " + ex?.Message,
                DescExcecao = ex?.StackTrace.Substring(0, 600)
            });
            _logContext.SaveChanges();
        }

        public ForecastET BuscarPorIdComIncludes(int idCelula, int idCliente, int idServico, int nrAno)
        {
            return _forecastRepository.BuscarPorIdComIncludes(idCelula, idCliente, idServico, nrAno);
        }

        public List<ComboClienteServicoDto> ObterServicoPorIdCelulaIdClienteEAcesso(int idCelula, int idCliente)
        {
            List<ComboClienteServicoDto> servicos = new List<ComboClienteServicoDto>();
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var query = "select distinct s.IdServico as id, s.Nome as descricao, s.SiglaTipoServico as Categoria from stfcorp.tblClientesServicos s, stfcorp.tblClientes c where s.IdCliente = " + idCliente + "and s.IdCelula = " + idCelula + " and  s.SiglaTipoServico NOT IN ('PROP', 'ACA', 'ACC', 'ACO') AND s.DtInativacao IS NULL ORDER BY descricao;";
                servicos = dbConnection.Query<ComboClienteServicoDto>(query).ToList();
                dbConnection.Close();
            }
            return servicos;
        }

        public void RealizarMigracaoBi()
        {
            List<ForecastDto> forecastsBi = ObterDadosForecastBiPlanilha();

            Console.WriteLine(forecastsBi.ToString());

            List<ValorForecast> forecasts = _valorForecastRepository.BuscarTodos().ToList();

            var countUpadte = 0;

            var countTotal = 0;

            forecastsBi.ForEach(f =>
            {
                var eforecastsExiste = forecasts.Any(e =>

                                    e.IdCelula == f.IdCelula &&
                                    e.IdCliente == f.IdCliente &&
                                    e.IdServico == f.IdServico &&
                                    e.NrAno == f.NrAno
                            );
                using (StreamWriter file =
                new StreamWriter(@"C:\Users\Duduulopes\Documents\Os Meus Ficheiros Recebidos\Result.txt", true))
                {
                    if (!eforecastsExiste)
                    {
                        file.WriteLine("Registro BI: ");
                        var t = "IdCelula =  " + f.IdCelula
                                + " and IdCliente =  " + f.IdCliente
                                + " and IdServico =  " + f.IdServico
                                + " and NrAno = " + f.NrAno;
                        file.WriteLine(t);
                    }
                    else
                    {
                        var forecastEncontrado = forecasts.Find(e =>

                                    e.IdCelula == f.IdCelula &&
                                    e.IdCliente == f.IdCliente &&
                                    e.IdServico == f.IdServico &&
                                    e.NrAno == f.NrAno
                                );
                        forecastEncontrado.ValorJaneiro = f.ValorJaneiro;
                        forecastEncontrado.ValorFevereiro = f.ValorFevereiro;
                        forecastEncontrado.ValorMarco = f.ValorMarco;
                        forecastEncontrado.ValorAbril = f.ValorAbril;
                        forecastEncontrado.ValorMaio = f.ValorMaio;
                        forecastEncontrado.ValorJunho = f.ValorJunho;
                        forecastEncontrado.ValorJulho = f.ValorJulho;
                        forecastEncontrado.ValorAgosto = f.ValorAgosto;
                        forecastEncontrado.ValorSetembro = f.ValorSetembro;
                        forecastEncontrado.ValorOutubro = f.ValorOutubro;
                        forecastEncontrado.ValorNovembro = f.ValorNovembro;
                        forecastEncontrado.ValorDezembro = f.ValorDezembro;
                        _valorForecastRepository.Update(forecastEncontrado);
                        countUpadte++;
                        countTotal++;
                        if (countUpadte == 100 || forecastsBi.Count == countTotal)
                        {
                            _unitOfWork.Commit();
                            countUpadte = 0;
                            file.WriteLine("Ultima alteração: ");
                            var t = "IdCelula =  " + f.IdCelula
                                + " and IdCliente =  " + f.IdCliente
                                + " and IdServico =  " + f.IdServico
                                + " and NrAno = " + f.NrAno;
                            file.WriteLine(t);
                        }
                    }
                }
            });
        }

        public int ObterQuantidadeDiasUteisAposViradaMes()
        {
            var listaFeriados = _calendarioService.ObterFeriadosNacionais(DateTime.Now.Year)
                .Where(d => d.Type.ToUpper() == "FERIADO NACIONAL");

            var diasUteis = 0;            

            var dataAtual = DateTime.Now;           
            var mesAtual = dataAtual.Month;

            DateTime diaAux = dataAtual.AddDays(-1);
            while (diaAux.Month == mesAtual)
            {
                if (diaAux.DayOfWeek != DayOfWeek.Saturday && diaAux.DayOfWeek != DayOfWeek.Sunday && !listaFeriados.Any(d => d.Date == diaAux))
                    diasUteis++;
                diaAux = diaAux.AddDays(-1);
            }

            return diasUteis;
        }

        private List<ForecastDto> ObterDadosForecastBiPlanilha()
        {
            var path = @"C:\Users\Duduulopes\Documents\Os Meus Ficheiros Recebidos\Forecast.xlsx";
            List<ForecastDto> forecastBiLista = new List<ForecastDto>();
            ForecastDto forecastBI = new ForecastDto();

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }

                var ws = pck.Workbook.Worksheets.First();

                var startRow = 2;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var valorJaneiro = ws.Cells[rowNum, 5, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(","-").Replace(")","").Trim();
                    var valorFevereiro = ws.Cells[rowNum, 6, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorMarco = ws.Cells[rowNum, 7, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorAbril = ws.Cells[rowNum, 8, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorMaio = ws.Cells[rowNum, 9, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorJunho = ws.Cells[rowNum, 10, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorJulho = ws.Cells[rowNum, 11, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorAgosto = ws.Cells[rowNum, 12, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorSetembro = ws.Cells[rowNum, 13, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorOutubro = ws.Cells[rowNum, 14, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();
                    var valorNovembro = ws.Cells[rowNum, 15, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim(); ;
                    var valorDezembro = ws.Cells[rowNum, 16, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text.Replace("(", "-").Replace(")", "").Trim();

                    //var Valor = (String.IsNullOrEmpty(valorJunho) || valorJunho == "-") ? 0 : Decimal.Parse(valorJunho);

                    forecastBI = new ForecastDto
                    {

                        IdCelula = Int32.Parse(ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text),
                        IdCliente = Int32.Parse(ws.Cells[rowNum, 2, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text),
                        IdServico = Int32.Parse(ws.Cells[rowNum, 3, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text),
                        NrAno = Int32.Parse(ws.Cells[rowNum, 4, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text),
                        ValorJaneiro = String.IsNullOrEmpty(valorJaneiro) || valorJaneiro == "-" ? 0 : Decimal.Parse(valorJaneiro),
                        ValorFevereiro = String.IsNullOrEmpty(valorFevereiro) || valorFevereiro == "-" ? 0 : Decimal.Parse(valorFevereiro),
                        ValorMarco = String.IsNullOrEmpty(valorMarco) || valorMarco == "-" ? 0 : Decimal.Parse(valorMarco),
                        ValorAbril = String.IsNullOrEmpty(valorAbril) || valorAbril == "-" ? 0 : Decimal.Parse(valorAbril),
                        ValorMaio = String.IsNullOrEmpty(valorMaio) || valorMaio == "-" ? 0 : Decimal.Parse(valorMaio),
                        ValorJunho = String.IsNullOrEmpty(valorJunho) || valorJunho == "-" ? 0 : Decimal.Parse(valorJunho),
                        ValorJulho = String.IsNullOrEmpty(valorJulho) || valorJulho == "-" ? 0 : Decimal.Parse(valorJulho),
                        ValorAgosto = String.IsNullOrEmpty(valorAgosto) || valorAgosto == "-" ? 0 : Decimal.Parse(valorAgosto),
                        ValorSetembro = String.IsNullOrEmpty(valorSetembro) || valorSetembro == "-" ? 0 : Decimal.Parse(valorSetembro),
                        ValorOutubro = String.IsNullOrEmpty(valorOutubro) || valorOutubro == "-" ? 0 : Decimal.Parse(valorOutubro),
                        ValorNovembro = String.IsNullOrEmpty(valorNovembro) || valorNovembro == "-" ? 0 : Decimal.Parse(valorNovembro),
                        ValorDezembro = String.IsNullOrEmpty(valorDezembro) || valorDezembro == "-" ? 0 : Decimal.Parse(valorDezembro),
                    };
                    forecastBiLista.Add(forecastBI);
                }
            }

            return forecastBiLista;
        }
    }
}
