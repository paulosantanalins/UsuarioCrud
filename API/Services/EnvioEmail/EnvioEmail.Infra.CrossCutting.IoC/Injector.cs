using EnvioEmail.Domain.EmailRoot.Repository;
using EnvioEmail.Domain.EmailRoot.Service;
using EnvioEmail.Domain.EmailRoot.Service.Interfaces;
using EnvioEmail.Domain.SharedRoot;
using EnvioEmail.Infra.Data.SqlServer.Repository;
using EnvioEmail.Infra.Data.SqlServer.UoW;
using Logger.Context;
using Logger.Repository;
using Logger.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using Utils;

namespace EnvioEmail.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Service
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITemplateEmailService, TemplateEmailService>();

            //Infra Data
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<ITemplateEmailRepository, TemplateEmailRepository>();
            services.AddScoped<IParametroTemplateRepository, ParametroTemplateRepository>();
            
            services.AddScoped<Variables>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            //log
            services.AddScoped<LogGenericoContext>();
            services.AddScoped<ILogGenericoRepository, LogGenericoRepository>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
