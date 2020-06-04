using FluentScheduler;
using GestaoServico.Api.Jobs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Salesforce.Models;
using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using GestaoServico.Infra.Data.SqlServer.Context;
using System.Text;
using Newtonsoft.Json;
using Logger.Context;
using Logger.Model;
using Utils.Extensions;
using DeveloperForce.NetCore.Force;
using DeveloperForce.NetCore.Common;
using Microsoft.AspNetCore.Hosting.Internal;
using Utils.Base;
using GestaoServico.Infra.CrossCutting.IoC;
using Utils;

namespace GestaoServico.Api.Jobs
{
    public class ObterContratosJob : IJob
    {
        private readonly object _lock = new object();
        private static HttpClient client = new HttpClient();
        private bool _shuttingDown;
        private List<ContratoSalesObject> TodosContratos = new List<ContratoSalesObject>();
        private List<AccountSalesObject> TodosClientes = new List<AccountSalesObject>();

        public ObterContratosJob()
        {
            // Register this job with the hosting environment.
            // Allows for a more graceful stop of the job, in the case of IIS shutting down.
            //HostingEnvironment.RegisterObject(this);
            //var token = lifeTime.ApplicationStopping.Register(OnApplicationStopping);
        }

        public void Execute()
        {
            try
            {
                lock (_lock)
                {
                    if (_shuttingDown)
                        return;

                    var auth = new AuthenticationClient();
                    auth.UsernamePasswordAsync(
                       "3MVG9Vik22TUgUpgCRwGDThV341ngTEs4NEwxdKpnnSgKQWWz8qdO.HuKgnMqmEYdhNkhCiQEPw6tZ87RyfRa",
                       "1880827973329100538",
                       "lfdamaceno1@stefanini.com",
                       "NogameNolife10" + "IQE5D5w03xW9MLWiJ6T15uoQr", "https://test.salesforce.com/services/oauth2/token").Wait();
                    var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
                    //if (VerificarExistenciaContratoBanco())
                    //{
                    //    TodosContratos = ObterContratosSalesForce(client); 
                    //}
                    //else
                    //{
                        TodosContratos = ObterTodosContratosSalesForce(client);
                    //}
                    foreach (var item in TodosContratos)
                    {
                        try
                        {
                            VerificarExistenciaContrato(item, client);
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                    }
                }
            }
            finally
            {
                // Always unregister the job when done.
                //HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            // Locking here will wait for the lock in Execute to be released until this code can continue.
            lock (_lock)
            {
                _shuttingDown = true;
            }

            //HostingEnvironment.UnregisterObject(this);
        }

        public void VerificarExistenciaContrato(ContratoSalesObject contratoSalesObject, ForceClient client)
        {
            var usuario = RecuperarVariablesToken();
            usuario.UserName = "salesForce";
            var _context = new GestaoServicoContext(usuario);
            var result = _context.Contratos.FirstOrDefault(x => x.NrAssetSalesForce == contratoSalesObject.ContractNumber);
            AdicionarLogGenerico("Inicio da persistencia do contrato de número: " + contratoSalesObject.ContractNumber);
            TratarContrato(result, contratoSalesObject, client, usuario);
        }

        private void TratarContrato(Contrato result, ContratoSalesObject contratoSalesObject, ForceClient client, IVariablesToken variables)
        {
            if (result == null)
            {
                var clienteSales = TodosClientes.FirstOrDefault(x => x.Id == contratoSalesObject.AccountId);
                if (clienteSales == null)
                {
                    clienteSales = ObterClienteSalesForcePorIdSalesForce(client, contratoSalesObject.AccountId);
                    TodosClientes.Add(clienteSales);
                }
                AdicionarLogGenerico("Busca do cliente: " + clienteSales.Name + " do contrato: " + contratoSalesObject.ContractNumber);
                contratoSalesObject.IdCliente = ObterClientePorIdSalesforce(clienteSales);
                if (contratoSalesObject.IdCliente == 0 && clienteSales.ParentId != null)
                {
                    var clientePaiSales = TodosClientes.FirstOrDefault(x => x.Id == clienteSales.ParentId);
                    if (clientePaiSales == null)
                    {
                        clientePaiSales = ObterClienteSalesForcePorIdSalesForce(client, clienteSales.ParentId);
                        TodosClientes.Add(clienteSales);
                    }
                    AdicionarLogGenerico("Busca do cliente mãe: " + clientePaiSales.Name + " do cliente: " + clienteSales.Name);
                    var idClientePai = ObterCliente(clientePaiSales);
                }
                contratoSalesObject.IdCliente = ObterCliente(clienteSales);
                result = Mapper.Map<Contrato>(contratoSalesObject);
            }
            else
            {

                if ((contratoSalesObject.EndDate.HasValue && result.DtFinalizacao != contratoSalesObject.EndDate.Value) || result.DescContrato != contratoSalesObject.N_mero_contrato_interno__c
                    || result.DescStatusSalesForce != contratoSalesObject.Status)
                {
                    AdicionarLogGenerico("Atualizando campos do contrato número: " + contratoSalesObject.ContractNumber);
                    result.DtFinalizacao = contratoSalesObject.EndDate;
                    result.DescStatusSalesForce = contratoSalesObject.Status;
                    result.DescContrato = contratoSalesObject.N_mero_contrato_interno__c;
                }
                else
                {
                    return;
                }
            }
            PersisitirContrato(result, variables);
        }

        private int ObterCliente(AccountSalesObject salesObject)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            var _microservicoUrls = RecuperarMicroServicosUrls();
            client.BaseAddress = new Uri(_microservicoUrls.UrlApiCliente);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(salesObject), Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/cliente/TratarClienteSalesforce", content).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AdicionarLogGenerico("Erro ao persistir cliente: " + salesObject.CNPJ__c);
            }
            else
            {
                AdicionarLogGenerico("Persistencia do cliente: " + salesObject.Name + " concluida.");
                var responseString = response.Content.ReadAsStringAsync().Result;
                return Int32.Parse(responseString);
            }
            return 0;
        }

        private int ObterClientePorIdSalesforce(AccountSalesObject salesObject)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            var _microservicoUrls = RecuperarMicroServicosUrls();
            client.BaseAddress = new Uri(_microservicoUrls.UrlApiCliente);


            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(salesObject), Encoding.UTF8, "application/json");
            var response = client.GetAsync("api/cliente/obterClientePorIdSalesforce/" + salesObject.Id).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Int32.Parse(responseString);
        }

        private List<ContratoSalesObject> ObterContratosSalesForce(ForceClient client)
        {
            var now = DateTime.Now;
            var dataFiltrar = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            string queryContrato = "SELECT Id, " +
                "AccountId, " +
                "StartDate, " +
                "EndDate, " +
                "ContractTerm, " +
                "OwnerId, " +
                "Status, " +
                "ActivatedById, " +
                "ActivatedDate, " +
                "StatusCode, " +
                "Description, " +
                "IsDeleted, " +
                "ContractNumber, " +
                "LastApprovedDate, " +
                "CreatedDate, " +
                "CreatedById, " +
                "LastModifiedDate, " +
                "LastModifiedById, " +
                "SystemModstamp, " +
                "LastActivityDate, " +
                "LastViewedDate, " +
                "LastReferencedDate, " +
                "N_mero_contrato_interno__c " +
                "FROM Contract " +
                "Where LastModifiedDate > " + dataFiltrar.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var results = client.QueryAsync<ContratoSalesObject>(queryContrato).Result;
            var totalSize = results.totalSize;
            return results.records;
        }

        private AccountSalesObject ObterClienteSalesForcePorIdSalesForce(ForceClient client, string idSalesForce)
        {
            string queryCliente = "SELECT " +
                "Id, " +
                "Name, " +
                "Type, " +
                "ParentId, " +
                "Phone, " +
                "Fax, " +
                "Website, " +
                "Industry, " +
                "Description, " +
                "OwnerId, " +
                "CreatedDate, " +
                "CreatedById, " +
                "LastModifiedDate, " +
                "LastModifiedById, " +
                "LastActivityDate, " +
                "LastViewedDate, " +
                "LastReferencedDate, " +
                "AccountSource, " +
                "SicDesc, " +
                "Setor_Old__c, " +
                "CNPJ__c, " +
                "NomeFantasia__c, " +
                "RazaoSocial__c, " +
                "RamoOld__c, " +
                "InscricaoEstadual__c, " +
                "InscricaoMunicipal__c, " +
                "Descricao__c, " +
                "TipoDeEndereco__c, " +
                "Logradouro__c, " +
                "NumeroEndereco__c, " +
                "ComplementoEndereco__c, " +
                "BairroEndereco__c, " +
                "CEPEndereco__c, " +
                "PaisDoProprietario__c, " +
                "Quantidade_de_oportunidades__c, " +
                "CidadeEstado__c, " +
                "PaisConta__c, " +
                "Neg_cios_Fechados_este_ano__c, " +
                "EstadoConta__c, Ramo__c, " +
                "StatusConta__c, " +
                "DonoDaConta__c, " +
                "Quantidade_de_Oportunidades_Ganhas__c, " +
                "de_fechamento__c, " +
                "TipoDeConta__c, " +
                "SAP__c, " +
                "OLD_ID__c, " +
                "Conta_Global__c, " +
                "Target__c, " +
                "Regi_o__c, " +
                "Grupo_Global__c, " +
                "CSM__c, " +
                "E_mail__c, " +
                "Data_Origem__c, " +
                "E_mail_do_propriet_rio__c, " +
                "Account_Status__c, " +
                "Governo__c, " +
                "eAcessoId__c, " +
                "Company_Size__c, " +
                "Or_amento_anual_de_TI__c, " +
                "DSCORGPKG__Conflict__c, " +
                "DSCORGPKG__DeletedFromDiscoverOrg__c, " +
                "Foco_Global__c, " +
                "planejamento_account__c, " +
                "Possui_Account_Planning__c, " +
                "Nome_Fantasia_Global__c, " +
                "UniqueEntry__Account_Dupes_Ignored__c, " +
                "Situa_o_cadastral__c, " +
                "Propriet_rio_do_registro__c, " +
                "Parental_ID__c, " +
                "CNPJ_Bairro__c, " +
                "CNPJ_CEP__c, " +
                "CNPJ_Cidade__c, " +
                "CNPJ_Complemento__c, " +
                "CNPJ_Endereco_Numero__c, " +
                "CNPJ_Estado__c, " +
                "CNPJ_Logradouro__c, " +
                "CNPJ_Situacao__c, " +
                "Phone_2__c, " +
                "Soma_de_todas_oportunidades__c " +
                "FROM Account " +
                "Where Id = '" + idSalesForce + "'";
            var resultsCliente = client.QueryAsync<AccountSalesObject>(queryCliente).Result;
            return resultsCliente.records.FirstOrDefault();
        }

        private void PersisitirContrato(Contrato result, IVariablesToken variables)
        {
            try
            {
                var _context = new GestaoServicoContext(variables);
                result.Usuario = "salesforce";
                result.DataAlteracao = DateTime.Now;
                if (result.Id == 0)
                {
                    _context.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }
                else
                {
                    _context.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                _context.SaveChanges();
                AdicionarLogGenerico("Persistido contrato número: " + result.NrAssetSalesForce);
            }
            catch (Exception)
            {
                AdicionarLogGenerico("Erro ao persistir contrato número: " + result.NrAssetSalesForce);
            }
        }

        private void AdicionarLogGenerico(string descricaoLog )
        {
            var _logContext = new LogGenericoContext();
            _logContext.LogGenericos.Add(new LogGenerico { NmTipoLog = Tipo.INTEGRACAO.GetDescription(), NmOrigem = Origem.SALESFORCE.GetDescription(), DtHoraLogGenerico = DateTime.Now, DescLogGenerico = descricaoLog, DescExcecao = "" });
            _logContext.SaveChanges();
        }

        private List<ContratoSalesObject> ObterTodosContratosSalesForce(ForceClient client)
        {
            var now = DateTime.Now;
            var dataFiltrar = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            string queryContrato = "SELECT Id, AccountId, StartDate, EndDate, ContractTerm, OwnerId, Status, ActivatedById, ActivatedDate, StatusCode, Description, IsDeleted, ContractNumber, LastApprovedDate, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById, SystemModstamp, LastActivityDate, LastViewedDate, LastReferencedDate, N_mero_contrato_interno__c FROM Contract ";
            var results = client.QueryAsync<ContratoSalesObject>(queryContrato).Result;
            return results.records;
        }

        private List<AccountSalesObject> ObterClienteSalesForce(ForceClient client)
        {
            string queryCliente = "SELECT Id, Name, Type, ParentId, Phone, Fax, Website, Industry, Description, OwnerId, CreatedDate, CreatedById, LastModifiedDate, LastModifiedById, LastActivityDate, LastViewedDate, LastReferencedDate, AccountSource, SicDesc, Setor_Old__c, CNPJ__c, NomeFantasia__c, RazaoSocial__c, RamoOld__c, InscricaoEstadual__c, InscricaoMunicipal__c, Descricao__c, TipoDeEndereco__c, Logradouro__c, NumeroEndereco__c, ComplementoEndereco__c, BairroEndereco__c, CEPEndereco__c, PaisDoProprietario__c, Quantidade_de_oportunidades__c, CidadeEstado__c, PaisConta__c, Neg_cios_Fechados_este_ano__c, EstadoConta__c, Ramo__c, StatusConta__c, DonoDaConta__c, Quantidade_de_Oportunidades_Ganhas__c, de_fechamento__c, TipoDeConta__c, SAP__c, OLD_ID__c, Conta_Global__c, Target__c, Regi_o__c, Grupo_Global__c, CSM__c, E_mail__c, Data_Origem__c, E_mail_do_propriet_rio__c, Account_Status__c, Governo__c, eAcessoId__c, Company_Size__c, Or_amento_anual_de_TI__c, DSCORGPKG__Conflict__c, DSCORGPKG__DeletedFromDiscoverOrg__c,Foco_Global__c, planejamento_account__c, Possui_Account_Planning__c, Nome_Fantasia_Global__c, UniqueEntry__Account_Dupes_Ignored__c, Situa_o_cadastral__c, Propriet_rio_do_registro__c, Parental_ID__c, CNPJ_Bairro__c, CNPJ_CEP__c, CNPJ_Cidade__c, CNPJ_Complemento__c, CNPJ_Endereco_Numero__c, CNPJ_Estado__c, CNPJ_Logradouro__c, CNPJ_Situacao__c, Phone_2__c, Soma_de_todas_oportunidades__c FROM Account";
            var resultsCliente = client.QueryAsync<AccountSalesObject>(queryCliente).Result;
            return resultsCliente.records;
        }

        private static MicroServicosUrls RecuperarMicroServicosUrls()
        {
            var microServicosUrls = Injector.ServiceProvider.GetService(typeof(MicroServicosUrls)) as MicroServicosUrls;
            return microServicosUrls;
        }

        private static IVariablesToken RecuperarVariablesToken()
        {
            var variablesToken = Injector.ServiceProvider.GetService(typeof(IVariablesToken)) as IVariablesToken;
            return variablesToken;
        }
    }
}
