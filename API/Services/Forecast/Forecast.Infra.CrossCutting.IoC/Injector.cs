using Forecast.Domain.ForecastRoot.Repository;
using Forecast.Domain.ForecastRoot.Service;
using Forecast.Domain.ForecastRoot.Service.Interfaces;
using Forecast.Domain.SharedRoot;
using Forecast.Infra.Data.SqlServer;
using Forecast.Infra.Data.SqlServer.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using Utils;
using Utils.Calendario;

namespace Forecast.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Service
            services.AddScoped<IForecastService, ForecastService>();

            //Infra Data
            services.AddScoped<IForecastRepository, ForecastRepository>();
            services.AddScoped<IValorForecastRepository, ValorForecastRepository>();

            //Globais
            services.AddScoped<Variables>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            //Utils
            services.AddScoped<ICalendarioService, CalendarioService>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
