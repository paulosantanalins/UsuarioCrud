using Microsoft.Extensions.DependencyInjection;
using ControleAcesso.Infra.CrossCutting.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
           Injector.RegisterServices(services);
        }
    }
}
