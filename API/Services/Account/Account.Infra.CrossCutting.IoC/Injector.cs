using Microsoft.Extensions.DependencyInjection;
using Account.Domain.Core.Notifications;
using Account.Infra.Data.SqlServer.Context;
using System;
using Account.Domain.ProjetoRoot.Service;
using Account.Domain.ProjetoRoot.Service.Interfaces;
using Account.Domain.ProjetoRoot.Repository;
using Account.Infra.Data.SqlServer.Repository;
using Utils;
using Logger.Repository.Interfaces;
using Logger.Repository;
using Logger.Context;
using Microsoft.AspNetCore.Http;

namespace Account.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //Domain Service
            services.AddScoped<ISistemaService, SistemaService>();

            //Infra Data
            services.AddScoped<ISistemaRepository, SistemaRepository>();

            services.AddScoped<NotificationHandler>();
            services.AddScoped<Variables>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<LogGenericoContext>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
