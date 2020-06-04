using Cadastro.Infra.CrossCutting.IoC;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Utils.Base;

namespace Cadastro.Api.Jobs
{
    public class AtualizarInformacaoCelulaJob
    {
        public void Execute()
        {
            var _microServicosUrls = RecuperarMicroservicosUrls();

            var client = new HttpClient
            {
                BaseAddress = new Uri(_microServicosUrls.UrlApiControle)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync("api/Migracao/atualizar-migracao/celula/1234").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
        }

        private static MicroServicosUrls RecuperarMicroservicosUrls()
        {
            var microServicosUrls = Injector.ServiceProvider.GetService(typeof(MicroServicosUrls)) as MicroServicosUrls;
            return microServicosUrls;
        }
    }
}
