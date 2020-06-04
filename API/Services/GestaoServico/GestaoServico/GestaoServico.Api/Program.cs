using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace GestaoServico.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseKestrel()
                .UseStartup<Startup>()
                //.UseUrls("http://0.0.0.0:17073")
                .Build();

                //HML
                //.UseKestrel()
                //.UseStartup<Startup>()
                //.UseUrls("http://0.0.0.0:7093")
                //.Build();
    }
}