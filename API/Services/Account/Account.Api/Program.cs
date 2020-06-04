using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Account.Api
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
                //.UseUrls("http://0.0.0.0:7071")
                .Build();

                //HML
                //.UseKestrel()
                //.UseStartup<Startup>()
                //.UseUrls("http://0.0.0.0:7091")
                //.Build();
    }
}
