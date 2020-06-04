using FluentScheduler;
using GestaoServico.Infra.CrossCutting.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Connections;
using Utils.StfAnalitcsDW.Model;
using Utils.StfAnalitcsDW.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using Utils.Base;

namespace GestaoServico.Api.Jobs
{
    public class MigrarServicosEacessoJob : IJob
    {
        public void Execute()
        {
            try
            {
                var servicos = ObterServicos();
                TratarServicos(servicos);
            }
            catch(Exception e)
            {
                // Always unregister the job when done.
                //HostingEnvironment.UnregisterObject(this);
            }
        }

        private void TratarServicos(List<ViewServicoModel> servicos)
        {
            var contratoService = RecuperarContratoService();
            var servicoContratadoService = RecuperarServicoContratadoService();
            var deParaService = RecuperarDeParaServicoService();

            foreach (var viewServico in servicos)
            {
                if (deParaService.VerificarIdServicoEacessoExistente(viewServico.IdServico))
                {
                    continue;
                }
                var servicoContratado = Mapper.Map<ServicoContratado>(viewServico);
                //servicoContratado.VinculoMarkupServicosContratados.Add(new VinculoMarkupServicoContratado
                //{
                //    VlMarkup = viewServico.Markup,
                //    DtFimVigencia = viewServico.DtFimVigencia,
                //    DtInicioVigencia = viewServico.DtInicioVigencia,
                //    Usuario = "Eacesso",
                //    DataAlteracao = DateTime.Now
                //});

                servicoContratado.DeParaServicos.Add(new DeParaServico
                {
                    DescStatus = "MA",
                    DescTipoServico = viewServico.NomeServico.Count() > 6 ? viewServico.NomeServico.Substring(0,6) : viewServico.NomeServico,
                    IdServicoEacesso = viewServico.IdServico,
                    DataAlteracao = DateTime.Now,
                    Usuario = "Eacesso"
                });
                viewServico.IdCliente = ObterClienteEacessoPorId(viewServico.IdCliente);
                servicoContratado.IdContrato = contratoService.VerificarExistenciaContratoEacesso(viewServico.IdContrato, viewServico.IdCliente);

                servicoContratadoService.Persistir(servicoContratado, servicoContratado.IdCelula, viewServico.Markup);

            }
        }

        private List<ViewServicoModel> ObterServicos()
        {
            var connections = RecuperarImportacaoServiceVariables();
            var dwService = new ViewServicoService(connections.DWConnection);
            return dwService.ObterServicos();
        }

        private int ObterClienteEacessoPorId(int idCliente)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(120);
            var _microServicoUrls = RecuperarMicroServicosUrls();
            client.BaseAddress = new Uri(_microServicoUrls.UrlApiCliente);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/cliente/" + idCliente + "/obter-cliente-migrado/").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Int32.Parse(responseString);
        }

        private static ConnectionStrings RecuperarImportacaoServiceVariables()
        {
            var connectionStrings = Injector.ServiceProvider.GetServices(typeof(ConnectionStrings)) as ConnectionStrings;
            return connectionStrings;
        }

        private static MicroServicosUrls RecuperarMicroServicosUrls()
        {
            var urls = Injector.ServiceProvider.GetServices(typeof(MicroServicosUrls)) as MicroServicosUrls;
            return urls;
        }

        private static IContratoService RecuperarContratoService()
        {
            var contratoService = Injector.ServiceProvider.GetServices(typeof(IContratoService)).FirstOrDefault() as IContratoService;
            return contratoService;
        }

        private static IServicoContratadoService RecuperarServicoContratadoService()
        {
            var servicoContratadoService = Injector.ServiceProvider.GetServices(typeof(IServicoContratadoService)).FirstOrDefault() as IServicoContratadoService;
            return servicoContratadoService;
        }

        private static IDeParaServicoService RecuperarDeParaServicoService()
        {
            var deParaServicoService = Injector.ServiceProvider.GetServices(typeof(IDeParaServicoService)).FirstOrDefault() as IDeParaServicoService;
            return deParaServicoService;
        }
    }
}
