using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Seguranca.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            //DEV    
            //.UseKestrel()
            .UseStartup<Startup>()
            //.UseUrls("http://0.0.0.0:7072")
            .Build();
    }
}
