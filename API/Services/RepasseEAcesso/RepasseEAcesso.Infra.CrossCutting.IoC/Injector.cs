using Microsoft.Extensions.DependencyInjection;
using Profissionais.Domain.BeneficioRoot.Service;
using Profissionais.Domain.BeneficioRoot.Service.Interfaces;
using RepasseEAcesso.Domain.DominioRoot.Repository;
using RepasseEAcesso.Domain.DominioRoot.Service;
using RepasseEAcesso.Domain.DominioRoot.Service.Interface;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Repository;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Services.Interfaces;
using RepasseEAcesso.Domain.RepasseRoot.Repository;
using RepasseEAcesso.Domain.RepasseRoot.Service;
using RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces;
using RepasseEAcesso.Domain.SharedRoot.Repository;
using RepasseEAcesso.Domain.SharedRoot.Service;
using RepasseEAcesso.Domain.SharedRoot.Service.Interface;
using RepasseEAcesso.Domain.SharedRoot.UoW;
using RepasseEAcesso.Domain.SharedRoot.UoW.Interfaces;
using RepasseEAcesso.Infra.Data.SqlServer.Context;
using RepasseEAcesso.Infra.Data.SqlServer.Repository;
using RepasseEAcesso.Infra.Data.SqlServer.Repository.DominioRepository;
using RepasseEAcesso.Infra.Data.SqlServer.Repository.LogRepasseRepository;
using RepasseEAcesso.Infra.Data.SqlServer.Repository.PeriodoRepasseRepository;
using RepasseEAcesso.Infra.Data.SqlServer.Repository.RepasseRepository;
using System;
using Microsoft.AspNetCore.Http;
using Utils;
using Utils.Calendario;

namespace RepasseEAcesso.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Infra Data
            services.AddScoped<RepasseEAcessoContext>();
            services.AddScoped<RepasseLegadoContext>();

            //Services
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IPeriodoRepasseService, PeriodoRepasseService>();
            services.AddScoped<IRepasseService, RepasseService>();
            services.AddScoped<IParametroBeneficioNatcorpService, ParametroBeneficioNatcorpService>();

            services.AddScoped<IRepasseNivelUmService, RepasseNivelUmService>();
            services.AddScoped<IDominioService, DominioService>();
            services.AddScoped<ILogRepasseService, LogRepasseService>();
            services.AddScoped<ICalendarioService, CalendarioService>();

            //Repository
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IPeriodoRepasseRepository, PeriodoRepasseRepository>();
            services.AddScoped<IRepasseNivelUmRepository, RepasseNivelUmRepository>();
            services.AddScoped<IDominioRepository, DominioRepository>();
            services.AddScoped<IRepasseRepository, RepasseRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IClienteServicoRepository, ClienteServicoRepository>();
            services.AddScoped<IProfissionaisRepository, ProfissionaisRepository>();
            services.AddScoped<IDominioService, DominioService>();
            services.AddScoped<ILogRepasseRepository, LogRepasseRepository>();

            //Globals
            services.AddScoped<Variables>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
