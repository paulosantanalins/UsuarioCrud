﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Forecast.Api
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
                //.UseUrls("http://0.0.0.0:7085")
                .Build();
    }
}
