using Microsoft.Extensions.DependencyInjection;
using Cliente.Domain.Core.Notifications;
using Cliente.Infra.Data.SqlServer.Context;
using Cliente.Infra.Data.SqlServer.Repository;
using System;
using Cliente.Domain.ClienteRoot.Service.Interfaces;
using Cliente.Domain.ClienteRoot.Service;
using Cliente.Domain.ClienteRoot.Repository;
using Logger.Repository.Interfaces;
using Logger.Repository;
using Logger.Context;
using Utils;
using Cliente.Domain.Interfaces;
using Cliente.Domain.SharedRoot;
using Cliente.Infra.Data.SqlServer.UoW;
using Microsoft.AspNetCore.Http;

namespace Cliente.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void RegisterServices(IServiceCollection services)
        {
            //Domain Service
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IGrupoClienteService, GrupoClienteService>();
            services.AddScoped<IEstadoService, EstadoService>();
            services.AddScoped<ICidadeService, CidadeService>();
            services.AddScoped<IEnderecoService, EnderecoService>();

            //Infra Data
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IGrupoClienteRepository, GrupoClienteRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IPaisRepository, PaisRepository>();
            services.AddScoped<IEstadoRepository, EstadoRepository>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();

            services.AddScoped<NotificationHandler>();
            services.AddScoped<Variables>();

            //log
            services.AddScoped<LogGenericoContext>();
            services.AddScoped<ILogGenericoRepository, LogGenericoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
