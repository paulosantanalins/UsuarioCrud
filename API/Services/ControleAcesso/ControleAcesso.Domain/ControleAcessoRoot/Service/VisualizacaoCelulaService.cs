using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ControleAcesso.Domain.SharedRoot;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace ControleAcesso.Domain.ControleAcessoRoot.Service
{
    public class VisualizacaoCelulaService : IVisualizacaoCelulaService
    {
        protected readonly IVisualizacaoCelulaRepository _visualizacaoCelulaRepository;
        private readonly ICelulaService _celulaService;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IVariablesToken _variables;
        private readonly MicroServicosUrls _microServicosUrls;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public VisualizacaoCelulaService(IVisualizacaoCelulaRepository visualizacaoCelulaRepository,
                                IUnitOfWork unitOfWork,
                                ICelulaService celulaService,
                                IOptions<ConnectionStrings> connectionStrings,
                                MicroServicosUrls microServicosUrls,
                                IVariablesToken variables)
        {
            _visualizacaoCelulaRepository = visualizacaoCelulaRepository;
            _unitOfWork = unitOfWork;
            _celulaService = celulaService;
            _connectionStrings = connectionStrings;
            _microServicosUrls = microServicosUrls;
            _variables = variables;
        }

        public void PersistirVisualizacaoCelula(List<VisualizacaoCelula> visualizacaoCelula)
        {
            var vinculosDB = _visualizacaoCelulaRepository.Buscar(x => visualizacaoCelula.Any(z => x.LgUsuario.Equals(z.LgUsuario))).ToList();

            if (vinculosDB.Any())
            {
                _visualizacaoCelulaRepository.RemoveRange(vinculosDB);
            }

            AdicionarVisualizacao(visualizacaoCelula);

            if (AtualizarEacesso(visualizacaoCelula))
            {
                _unitOfWork.Commit();
            }
            else
            {
                throw new Exception("Falha ao atualizar o Eacesso legado");
            }

        }


        public void AtualizarVisualizacaoCelula(List<VisualizacaoCelula> visualizacaoCelula)
        {
            var vinculosDB = _visualizacaoCelulaRepository.Buscar(x => x.LgUsuario.Trim().ToUpper()
                    .Equals(visualizacaoCelula.FirstOrDefault().LgUsuario.Trim().ToUpper())) as List<VisualizacaoCelula>;
            //var vinculosDB = _visualizacaoCelulaRepository.Buscar(x => visualizacaoCelula.Any(z => x.LgUsuario.TrimEnd().ToUpper().Equals(z.LgUsuario.TrimEnd().ToUpper()))) as List<VisualizacaoCelula>;
            if (vinculosDB.Any())
            {
                _visualizacaoCelulaRepository.RemoveRange(vinculosDB);
            }

            AdicionarVisualizacao(visualizacaoCelula);

            if (AtualizarEacesso(visualizacaoCelula))
            {
                _unitOfWork.Commit();
            }
            else
            {
                throw new Exception("Falha ao atualizar o Eacesso legado");
            }
        }

        public void RemoverVisualizacaoTodasCelulas(string login)
        {
            var vinculosDB = _visualizacaoCelulaRepository.Buscar(x => x.LgUsuario.Equals(login)).ToList();
            if (vinculosDB.Any())
            {
                _visualizacaoCelulaRepository.RemoveRange(vinculosDB);
            }

            if (AtualizarEacessoRemoverTodasCelulas(login))
            {
                _unitOfWork.Commit();
            }
            else
            {
                throw new Exception("Falha ao atualizar o Eacesso legado");
            }
        }


        private void AdicionarVisualizacao(List<VisualizacaoCelula> visualizacaoCelula)
        {
            var usuarios = visualizacaoCelula.Select(x => x.LgUsuario).Distinct();

            if (visualizacaoCelula.Any(x => x.TodasAsCelulasSempre || x.TodasAsCelulasSempreMenosAPropria))
            {
                foreach (var usuario in usuarios)
                {
                    VisualizacaoCelula visualizacao = new VisualizacaoCelula();
                    visualizacao.IdCelula = null;
                    visualizacao.TodasAsCelulasSempre = visualizacaoCelula.FirstOrDefault().TodasAsCelulasSempre;
                    visualizacao.TodasAsCelulasSempreMenosAPropria = visualizacaoCelula.FirstOrDefault().TodasAsCelulasSempreMenosAPropria;
                    visualizacao.LgUsuario = usuario;
                    _visualizacaoCelulaRepository.Adicionar(visualizacao);
                }
            }
            else
            {
                foreach (var visualizacao in visualizacaoCelula)
                {
                    _visualizacaoCelulaRepository.Adicionar(visualizacao);
                }
            }
        }

        private int ObterQuantidadeCelulas()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiServico);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/Celula/quantidade").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Int32.Parse(responseString);
        }

        private bool AtualizarEacesso(List<VisualizacaoCelula> visualizacaoCelula)
        {
            if (visualizacaoCelula.Any(x => x.TodasAsCelulasSempreMenosAPropria)) 
                visualizacaoCelula.RemoveAll(x => x.IdCelula == x.IdCelulaUsuarioVinculado);

            try
            {
                var connectionString = _connectionStrings.Value.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();

                    foreach (var item in visualizacaoCelula.GroupBy(x => x.LgUsuario))
                    {
                        string sQueryDelecao = "delete from [stfcorp].[EUSER_VisualizacaoCelulas] where login = '" + item.Key + "'";
                        dbConnection.Query(sQueryDelecao);
                    }

                    using (SqlBulkCopy copy = new SqlBulkCopy(dbConnection as SqlConnection))
                    {
                        copy.DestinationTableName = "[stfcorp].[EUSER_VisualizacaoCelulas]";
                        DataTable table = new DataTable("[stfcorp].[EUSER_VisualizacaoCelulas]");
                        table.Columns.Add("celula", typeof(int));
                        table.Columns.Add("login", typeof(string));
                        table.Columns.Add("logininclusao", typeof(string));
                        table.Columns.Add("dtinclusao", typeof(DateTime));

                        foreach (var item in visualizacaoCelula)
                        {
                            table.Rows.Add(item.IdCelula, item.LgUsuario, "STFCORP", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        copy.WriteToServer(table);
                    }
                 
                    dbConnection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool AtualizarEacessoRemoverTodasCelulas(string login)
        {
            try
            {
                var connectionString = _connectionStrings.Value.EacessoConnection;
                using (IDbConnection dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();

                    string sQueryDelecao = "delete from [stfcorp].[EUSER_VisualizacaoCelulas] where login = '" + login + "'";
                    dbConnection.Query(sQueryDelecao);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public List<VisualizacaoCelula> BuscarPorLogin(string login)
        {
            var result = _visualizacaoCelulaRepository.ObterVinculoPorLogin(login);
            return result;
        }

        public ICollection<VisualizacaoCelulaDto> ObterVisualizacaoCelularPorLogin(string login, int celulaUsuario = -1)
        {
            var visualizacoes = _visualizacaoCelulaRepository.ObterVisualizacaoCelularPorLogin(login).ToList();

            if (celulaUsuario == -1 && _variables.CelulaUsuario > -1)
            {
                celulaUsuario = _variables.CelulaUsuario;
            }

            if (visualizacoes.Any(x => x.TodasAsCelulasSempreMenosAPropria) && _variables.CelulaUsuario > -1)
            {
                visualizacoes.RemoveAll(x => x.Id == celulaUsuario);
            }

            return visualizacoes;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarUsuariosCelulaDropdown(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var result = _visualizacaoCelulaRepository.FiltrarUsuariosCelulaDropdown(filtro);
            return result;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> VisualizarUsuariosCelula(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var result = _visualizacaoCelulaRepository.VisualizarUsuariosCelula(filtro);
            return result;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> ObterUsuariosComVisualizacao(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var result = _visualizacaoCelulaRepository.ObterUsuariosComVisualizacao(filtro);
            return result;
        }


        public IEnumerable<UsuarioSegurancaDto> ObterUsuariosAdComFiltroCelula(FiltroAdDtoSeguranca filtro)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            client.BaseAddress = new Uri(_microServicosUrls.UrlApiSeguranca);
            //client.BaseAddress = new Uri("http://localhost:17069/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var json = JsonConvert.SerializeObject(filtro);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/Authentication/BuscarUsuariosAdPorCelula/", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var retorno = JsonConvert.DeserializeObject<RetornoSegurancaDto>(responseString);

            var visualizacoesPorCelula = _visualizacaoCelulaRepository.BuscarTodosPorLoginDistinct();

            var listaRetorno = retorno.Dados.Select(x => new UsuarioSegurancaDto
            {
                Login = x.Login,
                NomeCompleto = x.NomeCompleto,
                Celula = x.Celula,
                IdCelula = Convert.ToInt32(x.Celula.Split(' ')[1]),
                CPF = x.CPF,
                Email = x.Email,
                Cargo = x.Cargo,
                PossuiAlgumaVisualizacaoCelula = visualizacoesPorCelula.Any(y => y.LgUsuario.ToUpper() == x.Login.ToUpper())
            });
            return listaRetorno;
        }

        public List<UsuarioSegurancaDto> BuscarUsuariosAdPorCelulas(string celulas)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(120),             
                BaseAddress = new Uri(_microServicosUrls.UrlApiSeguranca)
               
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _variables.Token);
            var content = new StringContent(JsonConvert.SerializeObject(
                new FiltroAdDtoSeguranca { Celulas = celulas.Split(",").ToList() }), Encoding.UTF8, "application/json");

            var response = client.PostAsync("api/Authentication/BuscarUsuariosAdPorCelula", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var retornoSeguranca = JsonConvert.DeserializeObject<RetornoSegurancaDto>(responseString);

            return retornoSeguranca.Dados;
        }

        public FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> FiltrarGrid(FiltroGenericoDto<UsuarioVisualizacaoCelulaDto> filtro)
        {
            var result = _visualizacaoCelulaRepository.FiltrarGrid(filtro);
            return result;
        }

        public void RealizarMigracaoVisualizacaoCelula()
        {
            List<VisualizacaoCelula> visualizacaoEacesso;
            var connectionStringEacesso = _connectionStrings.Value.EacessoConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringEacesso))
            {
                string sQuery = " SELECT celula as IdCelula, login as LgUsuario, logininclusao as Usuario, dtinclusao as DataAlteracao " +
                    "from [stfcorp].[EUSER_VisualizacaoCelulas]";
                dbConnection.Open();
                visualizacaoEacesso = dbConnection.Query<VisualizacaoCelula>(sQuery).AsList();
                dbConnection.Close();
            }

            var agrupados = visualizacaoEacesso.GroupBy(x => x.LgUsuario);
            var full = agrupados.Where(x => x.Count() >= 500);
            var normais = agrupados.Where(x => x.Count() < 500);

            var connectionStringSTFCORP = Variables.DefaultConnection;
            using (IDbConnection dbConnection = new SqlConnection(connectionStringSTFCORP))
            {
                dbConnection.Open();
                foreach (var item in full)
                {
                    string sQueryInsert = "INSERT INTO TBLVISUALIZACAOCELULA (IDCELULA, LGUSUARIOVINCULADO, LGUSUARIO, DTALTERACAO) VALUES " +
                        "(null, '" + item.Key + "', 'EACESSO', '" + DateTime.Now.ToString("yyyy/MM/dd") + "')";
                    dbConnection.Query(sQueryInsert);
                }
                foreach (var item in normais)
                {
                    foreach (var visualizacao in item)
                    {
                        string sQueryInsert = "INSERT INTO TBLVISUALIZACAOCELULA (IDCELULA, LGUSUARIOVINCULADO, LGUSUARIO, DTALTERACAO) VALUES " +
                            "(" + visualizacao.IdCelula + ", '" + item.Key + "', '" + visualizacao.Usuario + "', '" + visualizacao.DataAlteracao.Value.ToString("yyyy/MM/dd") + "')";
                        dbConnection.Query(sQueryInsert);
                    }
                }
                dbConnection.Close();
            }
        }
    }


}
