using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace ControleAcesso.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile($"{Directory.GetCurrentDirectory()}/log.txt",
                        retainedFileCountLimit: 3)
                    .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //DEV
                //.UseKestrel()
                .UseStartup<Startup>()
                //.UseUrls("http://0.0.0.0:7081")
                .Build();

                //HML
                //.UseKestrel()
                //.UseStartup<Startup>()
                //.UseUrls("http://0.0.0.0:7092")
                //.Build();
    }
}
