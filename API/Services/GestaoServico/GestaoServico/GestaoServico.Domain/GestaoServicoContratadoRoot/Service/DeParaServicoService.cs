using Dapper;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Utils;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Domain.GestaoServicoContratadoRoot.Service
{
    public class DeParaServicoService : IDeParaServicoService
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly Variables _variables;
        private readonly IDeParaServicoRepository _deParaServicoRepository;
        private readonly IServicoContratadoService _servicoContratadoService;
        private readonly IContratoService _contratoService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IVinculoServicoCelulaComercialService _vinculoServicoCelulaComercialService;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IServicoEAcessoService _servicoEAcessoService;

        public DeParaServicoService(IUnitOfWork unitOfWork,
                                    Variables variables,
                                    IDeParaServicoRepository deParaServicoRepository,
                                    IContratoService contratoService,
                                    MicroServicosUrls microServicosUrls,
                                    IVinculoServicoCelulaComercialService vinculoServicoCelulaComercialService,
                                    IServicoContratadoService servicoContratadoService,
                                    IOptions<ConnectionStrings> connectionStrings, IServicoEAcessoService servicoEAcessoService)
        {
            _unitOfWork = unitOfWork;
            _variables = variables;
            _servicoContratadoService = servicoContratadoService;
            _vinculoServicoCelulaComercialService = vinculoServicoCelulaComercialService;
            _contratoService = contratoService;
            _microServicosUrls = microServicosUrls;
            _deParaServicoRepository = deParaServicoRepository;
            _connectionStrings = connectionStrings;
            _servicoEAcessoService = servicoEAcessoService;
        }


        public async Task<FiltroGenericoDtoBase<GridServicoMigradoDTO>> Filtrar(FiltroGenericoDtoBase<GridServicoMigradoDTO> filtro)
        {
            return _deParaServicoRepository.Filtrar(filtro);
        }


        public async Task ObterNomeServicoLegado(GridServicoMigradoDTO gridServicoMigradoDTO)
        {
            var clienteServicoService = new ClienteServicoEacessoService(_connectionStrings.Value.EacessoConnection);
            gridServicoMigradoDTO.NmServicoEacesso = (await clienteServicoService.ObterNomeServicoPorId(gridServicoMigradoDTO.IdServicoEacesso));
        }

        public bool VerificarIdServicoEacessoExistente(int idServico)
        {
            var result = _deParaServicoRepository.Buscar(x => x.IdServicoEacesso == idServico);

            return (result != null && result.Any());
        }


        public int BuscarIdServicoContratadoPorIdServicoEacesso(int idServicoEacesso)
        {
            return _deParaServicoRepository.ObterIdServicoContratadoPorIdServicoEacesso(idServicoEacesso);
        }

        public DeParaServico ObterServicoMigradoPorId(int id)
        {
            return _deParaServicoRepository.ObterDeParaPorId(id);
        }

        private void PopularVinculoComercial(IDbConnection dbConnection, int idServicoComercial, ServicoContratado servicoContratado)
        {
            servicoContratado.VinculoServicoCelulaComercial.Add(new VinculoServicoCelulaComercial
            {
                IdCelulaComercial = ObterCelulaPorServico(dbConnection, idServicoComercial),
                DataInicial = servicoContratado.DtInicial,
                DataFinal = servicoContratado.DtFinal
            });
        }

        private static void PopularDePara(int idServicoTecnico, int? idServicoComercial, ServicoContratado servicoContratado)
        {
            servicoContratado.DeParaServicos.Add(new DeParaServico
            {
                IdServicoEacesso = idServicoTecnico,
                DescStatus = "MA",
                DescTipoServico = ""
            });

            if (idServicoComercial.HasValue)
            {
                servicoContratado.DeParaServicos.Add(new DeParaServico
                {
                    IdServicoEacesso = idServicoComercial.Value,
                    DescStatus = "MA",
                    DescTipoServico = ""
                });
            }
        }

        private Contrato CriarContrato(IDbConnection dbConnection, ServicoContratado servicoContratado)
        {
            Contrato contrato = new Contrato
            {
                DtInicial = new DateTime(2019, 1, 1),
                DescContrato = "Contrato Default",
                DescStatusSalesForce = "N/A",
                IdMoeda = 1,
                DataAlteracao = DateTime.Now,
                Usuario = "STFCORP"
            };

            //var clienteSTFCORP = BuscarClienteStfcorpPorIdEacesso(servicoContratado.IdCliente);
            //if (clienteSTFCORP != null)
            //{
            //    contrato.IdCliente = clienteSTFCORP.Id;
            //}
            //else
            //{
            //    var clienteEAcesso = BuscarClientePorIdEacesso(dbConnection, servicoContratado.IdCliente);
            //    ClienteET cliente = new ClienteET
            //    {
            //        Id = servicoContratado.IdCliente,
            //        NrCnpj = clienteEAcesso.NrCnpj,
            //        NmRazaoSocial = clienteEAcesso.NmRazaoSocial,
            //        IdSalesforce = clienteEAcesso.IdSalesforce,
            //        FlTipoHierarquia = "M",
            //        FlStatus = "P",
            //        Usuario = "STFCORP",
            //        DataAlteracao = DateTime.Now
            //    };
            //    contrato.IdCliente = cliente.Id;
            //    contrato.Cliente = cliente;
            //}

            return contrato;
        }

        private ClienteET BuscarClienteStfcorpPorIdEacesso(int idClienteEAcesso)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            //client.BaseAddress = new Uri(_microServicosUrls.UrlApiCliente);
            client.BaseAddress = new Uri("https://stfcorp.stefanini.com/api/cliente/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/Cliente/id-eacesso" + idClienteEAcesso).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var retorno = JsonConvert.DeserializeObject<ClienteET>(responseString);
            return retorno;
        }

        private ClienteET BuscarClientePorIdEacesso(IDbConnection dbConnection, int idClienteEAcesso)
        {
            var query = @"select substring(cnpj,1,18) as nrcnpj, razaosocial as nmrazaosocial, salesforceid as idsalesforce from stfcorp.tblclientes where idCliente = " + idClienteEAcesso;
            var cliente = dbConnection.Query<ClienteET>(query).FirstOrDefault();
            return cliente;
        }

        private int ObterCelulaPorServico(IDbConnection dbConnection, int idServicoEAcesso)
        {
            var query = @"select idCelula from stfcorp.tblclientesservicos where idServico = " + idServicoEAcesso;
            var idCelula = dbConnection.Query<int>(query).FirstOrDefault();
            return idCelula;
        }

        private List<int> ObterCustosInternoViaPlanilha()
        {
            var path = @"C:\Users\Duduulopes\Downloads\Combinada.xlsx";
            List<int> custosInternos = new List<int>();

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }

                var ws = pck.Workbook.Worksheets.ElementAt(1);

                var startRow = 2;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var servicoCustoInterno = Int32.Parse(ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column].FirstOrDefault().Text);

                    custosInternos.Add(servicoCustoInterno);
                }
            }

            return custosInternos;
        }

        private List<CombinadaDTO> ObterCombinadasViaPlanilha()
        {
            var path = @"C:\Users\Duduulopes\Downloads\Combinada.xlsx";
            List<CombinadaDTO> combinadas = new List<CombinadaDTO>();
            CombinadaDTO combinada = new CombinadaDTO();

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
                    var servicoTecnico = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    var servicoComercial = ws.Cells[rowNum, 2, rowNum, ws.Dimension.End.Column];

                    combinada = new CombinadaDTO
                    {
                        IdServicoTecnico = Int32.Parse(servicoTecnico.FirstOrDefault().Text),
                        IdServicoComercial = Int32.Parse(servicoComercial.FirstOrDefault().Text)
                    };
                    combinadas.Add(combinada);
                }
            }

            return combinadas;
        }

        private List<int> ObterServicosComMovimentacaoPorPeriodoEdiretoria(string inicioPeriodo, string fimPeriodo, List<int> celulasDiretoria)
        {
            List<int> idServicosEAcesso;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                var servicosComRepasseNaDiretoria = ServicosComRepasseNaDiretoria(dbConnection, inicioPeriodo, fimPeriodo, celulasDiretoria);
                var servicosComRepasseEnvolvendoDiretoria = ServicosComRepasseEnvolvendoDiretoria(dbConnection, servicosComRepasseNaDiretoria, inicioPeriodo, fimPeriodo, celulasDiretoria);
                var servicosComMovimentacaoRM = ServicosComMovimentacaoRM(dbConnection, inicioPeriodo, fimPeriodo, celulasDiretoria);

                var todosServicos = servicosComRepasseNaDiretoria.Concat(servicosComRepasseEnvolvendoDiretoria.Concat(servicosComMovimentacaoRM)).ToList();
                idServicosEAcesso = todosServicos.GroupBy(x => x).Select(x => x.Key).ToList();

                dbConnection.Close();
            }

            return idServicosEAcesso;
        }

        private static List<int> ServicosComMovimentacaoRM(IDbConnection dbConnection, string inicioPeriodo, string fimPeriodo, List<int> celulasDiretoria)
        {
            string celulas = "";
            string celulaMontada = "";
            foreach (var celula in celulasDiretoria)
            {
                if (celulasDiretoria.LastOrDefault() != celula)
                {
                    celulaMontada = (("0000" + celula).Substring(("0000" + celula).Length - 4));
                    celulas += "RAT.CODCCUSTO LIKE '" + celulaMontada + ".%' or ";
                }
                else
                {
                    celulaMontada = (("0000" + celula).Substring(("0000" + celula).Length - 4));
                    celulas += "RAT.CODCCUSTO LIKE '" + celulaMontada + ".%'";
                }
            }

            string query = @"SELECT distinct RAT.CODCCUSTO AS CodigoCusto
                            FROM CorporeRM.DBO.FLANRATCCU as RAT 
                            INNER JOIN CorporeRM.DBO.FLAN AS LANCAMENTOS WITH(NOLOCK) 
                            ON LANCAMENTOS.IDLAN = RAT.IDLAN AND LANCAMENTOS.CODCOLIGADA = RAT.CODCOLIGADA 
                            LEFT JOIN CorporeRM.DBO.TMOV AS MOVIMENTOS WITH(NOLOCK) 
                            ON MOVIMENTOS.IDMOV = LANCAMENTOS.IDMOV 
                            AND MOVIMENTOS.CODCOLIGADA = LANCAMENTOS.CODCOLIGADA 
                            WHERE LANCAMENTOS.STATUSLAN IN(1,3) 
                            AND LANCAMENTOS.DATABAIXA BETWEEN CONVERT(DATETIME, '" + inicioPeriodo + "', 103) " +
                            " AND CONVERT(DATETIME, '" + fimPeriodo + "',103) " +
                            " AND LANCAMENTOS.CODCOLIGADA NOT IN (31,30,23)" +
                            " and LANCAMENTOS.IDLAN not in (select distinct(idlan) from [stfcorp].[EFATURAMENTO_Repasse_FLAN]) AND (" + celulas + ")";

            var centrosCustoMovimentados = dbConnection.Query<string>(query).ToList();

            List<int> servicos = new List<int>();
            foreach (var item in centrosCustoMovimentados)
            {
                var servico = Int32.Parse(item.Split('.')[2]);
                if (!servicos.Any(x => x == servico))
                {
                    servicos.Add(servico);
                }
            }

            return servicos;
        }

        private static List<int> ServicosComRepasseEnvolvendoDiretoria(IDbConnection dbConnection, IEnumerable<int> servicosDiretoria, string inicioPeriodo, string fimPeriodo, List<int> celulasDiretoria)
        {
            string query = @"SELECT distinct REPASSES.idServico AS IdServico FROM[STFCORP].efaturamento_repasse AS REPASSES
                            WHERE REPASSES.idorigem in (1, 3)
                                    AND REPASSES.DT_REPASSE BETWEEN '" + inicioPeriodo + "' AND '" + fimPeriodo + "'" +
                                    " AND REPASSES.cd_cel_origem not in (109) AND REPASSES.status_aprov IN('AP') " +
                                    " AND REPASSES.cd_cel_origem in (" + string.Join(",", celulasDiretoria) + ")";
            var servicosDestinoRepasse = dbConnection.Query<ServicosDeParaDto>(query).ToList();

            query = @"SELECT distinct REPASSES.idServico1 AS IdServico FROM[STFCORP].efaturamento_repasse AS REPASSES
                            WHERE REPASSES.idorigem in (1, 3)
                                    AND REPASSES.DT_REPASSE BETWEEN '" + inicioPeriodo + "' AND '" + fimPeriodo + "'" +
                                    " AND REPASSES.cd_cel_origem not in (109) AND REPASSES.status_aprov IN('AP') " +
                                    " AND REPASSES.cd_cel_destino in (" + string.Join(",", celulasDiretoria) + ")";
            var servicosOrigemRepasse = dbConnection.Query<ServicosDeParaDto>(query).ToList();

            var servicos = servicosOrigemRepasse.Concat(servicosDestinoRepasse).GroupBy(x => x.IdServico).Select(x => x.Key).ToList();
            return servicos.Where(x => !servicosDiretoria.Any(y => y == x)).ToList();
        }

        private static List<int> ServicosComRepasseNaDiretoria(IDbConnection dbConnection, string inicioPeriodo, string fimPeriodo, List<int> celulasDiretoria)
        {
            var query = @"SELECT distinct REPASSES.idServico1 AS IdServico FROM[STFCORP].efaturamento_repasse AS REPASSES
                            WHERE REPASSES.idorigem in (1, 3)
                                    AND REPASSES.DT_REPASSE BETWEEN '" + inicioPeriodo + "' AND '" + fimPeriodo + "'" +
                                    " AND REPASSES.cd_cel_origem not in (109) AND REPASSES.status_aprov IN('AP') " +
                                    " AND REPASSES.cd_cel_origem in (" + string.Join(",", celulasDiretoria) + ")";
            var servicosOrigemRepasseDiretoria = dbConnection.Query<ServicosDeParaDto>(query).ToList();

            query = @"SELECT distinct REPASSES.idServico AS IdServico FROM[STFCORP].efaturamento_repasse AS REPASSES
                            WHERE REPASSES.idorigem in (1, 3)
                                    AND REPASSES.DT_REPASSE BETWEEN '" + inicioPeriodo + "' AND '" + fimPeriodo + "'" +
                                    " AND REPASSES.cd_cel_origem not in (109) AND REPASSES.status_aprov IN('AP') " +
                                    " AND REPASSES.cd_cel_destino in (" + string.Join(",", celulasDiretoria) + ")";
            var servicosDestinoRepasseDiretoria = dbConnection.Query<ServicosDeParaDto>(query).ToList();
            return servicosOrigemRepasseDiretoria.Concat(servicosDestinoRepasseDiretoria).GroupBy(x => x.IdServico).Select(x => x.Key).ToList();
        }

        public void MigrarServicos()
        {
            const string INICIO_PERIODO = "2019-01-01 00:00:00";
            const string FIM_PERIODO = "2019-07-01 00:00:00";
            List<int> CELULAS_DIRETORIA = new List<int> { 30, 37, 47, 75, 81, 127, 190, 211, 293, 346, 364, 366, 369, 386, 435, 445, 577 };

            var combinadas = ObterCombinadasViaPlanilha();
            var custosInternos = ObterCustosInternoViaPlanilha();

            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                dbConnection.Open();
                PersistirCombinadas(combinadas, dbConnection);
                PersistirCustoInterno(custosInternos, dbConnection);
                RealizarDeParaEmServicosForaDiretoria(dbConnection, INICIO_PERIODO, FIM_PERIODO, CELULAS_DIRETORIA);
                dbConnection.Close();
            }
        }

        private void RealizarDeParaEmServicosForaDiretoria(IDbConnection dbConnection, string inicioPeriodo, string fimPeriodo, List<int> celulasDiretoria)
        {
            var idsServicosFaltantes = ObterServicosFaltantes(inicioPeriodo, fimPeriodo, celulasDiretoria);
            var servicosEAcessoForaDiretoria = _servicoEAcessoService.ObterServicosForaDiretoria(dbConnection, idsServicosFaltantes, celulasDiretoria);
            List<DeParaServico> deParaServicos = new List<DeParaServico>();
            foreach (var idEAcesso in servicosEAcessoForaDiretoria)
            {
                var dePara = new DeParaServico
                {
                    IdServicoEacesso = idEAcesso,
                    DescStatus = "MA",
                    DescTipoServico = "",
                    IdServicoContratado = 2233
                };
                deParaServicos.Add(dePara);
            }
            _deParaServicoRepository.AdicionarRange(deParaServicos);
            _unitOfWork.Commit();
        }

        private void PersistirCombinadas(List<CombinadaDTO> combinadas, IDbConnection dbConnection)
        {
            foreach (var combinada in combinadas)
            {
                if (!_deParaServicoRepository.Buscar(x => x.IdServicoEacesso == combinada.IdServicoTecnico).Any())
                {
                    CriarServicoContratadoNovo(dbConnection, combinada);
                }
                else
                {
                    CriarApenasRelacao(dbConnection, combinada);
                }
            }
        }

        private void CriarApenasRelacao(IDbConnection dbConnection, CombinadaDTO combinada)
        {
            var servicoContratado = _servicoEAcessoService.BuscarServicoEAcesso(dbConnection, combinada.IdServicoTecnico);
            var deParaAntigo = _deParaServicoRepository.Buscar(x => x.IdServicoEacesso == combinada.IdServicoTecnico).FirstOrDefault();
            var vinculo = new VinculoServicoCelulaComercial
            {
                IdCelulaComercial = ObterCelulaPorServico(dbConnection, combinada.IdServicoComercial),
                DataInicial = servicoContratado.DtInicial,
                DataFinal = servicoContratado.DtFinal,
                IdServicoContratado = deParaAntigo.IdServicoContratado
            };
            _vinculoServicoCelulaComercialService.Adicionar(vinculo);

            var deParaNovo = new DeParaServico
            {
                IdServicoEacesso = combinada.IdServicoComercial,
                IdServicoContratado = deParaAntigo.IdServicoContratado,
                DescStatus = "MA",
                DescTipoServico = ""
            };
            _deParaServicoRepository.Adicionar(deParaNovo);
            _unitOfWork.Commit();
        }

        private void CriarServicoContratadoNovo(IDbConnection dbConnection, CombinadaDTO combinada)
        {
            var servicoContratado = _servicoEAcessoService.BuscarServicoEAcesso(dbConnection, combinada.IdServicoTecnico);
            var contratos = _contratoService.ObterContratosPorCliente(servicoContratado.IdCliente);
            if (contratos.Any())
            {
                servicoContratado.IdContrato = contratos.FirstOrDefault().Id;
            }
            else
            {
                Contrato contrato = CriarContrato(dbConnection, servicoContratado);
                servicoContratado.Contrato = contrato;
            }

            PopularDePara(combinada.IdServicoTecnico, combinada.IdServicoComercial, servicoContratado);
            PopularVinculoComercial(dbConnection, combinada.IdServicoComercial, servicoContratado);

            _servicoContratadoService.Criar(servicoContratado);
        }

        private void PersistirCustoInterno(List<int> custosInternos, IDbConnection dbConnection)
        {
            foreach (var idServico in custosInternos)
            {
                if (!_deParaServicoRepository.Buscar(x => x.IdServicoEacesso == idServico).Any())
                {
                    var servicoContratado = _servicoEAcessoService.BuscarServicoEAcesso(dbConnection, idServico);
                    var contratos = _contratoService.ObterContratosPorCliente(servicoContratado.IdCliente);
                    if (contratos.Any())
                    {
                        servicoContratado.IdContrato = contratos.FirstOrDefault().Id;
                    }
                    else
                    {
                        Contrato contrato = CriarContrato(dbConnection, servicoContratado);
                        servicoContratado.Contrato = contrato;
                    }

                    PopularDePara(idServico, null, servicoContratado);

                    _servicoContratadoService.Criar(servicoContratado);
                }
                else
                {
                    var teste = "";
                }
            }
        }

        public void MigrarMovimentacao()
        {
            const string INICIO_PERIODO = "2019-01-01 00:00:00";
            const string FIM_PERIODO = "2019-07-01 00:00:00";
            List<int> CELULAS_DIRETORIA = new List<int> { 30, 37, 47, 75, 81, 127, 190, 211, 293, 346, 364, 366, 369, 386, 435, 445, 577 };

            List<int> idServicoFaltantes = ObterServicosFaltantes(INICIO_PERIODO, FIM_PERIODO, CELULAS_DIRETORIA);
            var clauseIn = string.Join(",", idServicoFaltantes);
        }

        private List<int> ObterServicosFaltantes(string inicioPeriodo, string fimPeriodo, List<int> celulasDiretoria)
        {
            List<int> idServicosEAcesso = ObterServicosComMovimentacaoPorPeriodoEdiretoria(inicioPeriodo, fimPeriodo, celulasDiretoria).OrderBy(x => x).ToList();
            var servicosRelacionados = _deParaServicoRepository.BuscarTodos().ToList();
            var idServicoFaltantes = idServicosEAcesso.Where(x => !servicosRelacionados.Any(y => y.IdServicoEacesso == x)).ToList();
            return idServicoFaltantes;
        }
    }
}
