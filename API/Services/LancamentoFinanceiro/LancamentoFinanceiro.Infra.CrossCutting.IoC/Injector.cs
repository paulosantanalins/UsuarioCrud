using Microsoft.Extensions.DependencyInjection;
using LancamentoFinanceiro.Domain.Core.Notifications;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using LancamentoFinanceiro.Infra.Data.SqlServer.Repository;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service.Interfaces;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Repository;
using LancamentoFinanceiro.Infra.Data.SqlServer.UoW;
using LancamentoFinanceiro.Domain;
using Utils;
using Logger.Repository;
using Logger.Repository.Interfaces;
using Logger.Context;
using System;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Service.Interfaces;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Service;
using LancamentoFinanceiro.Domain.MigracaoRMRoot.Service.Interface;
using LancamentoFinanceiro.Domain.MigracaoRMRoot.Service;
using Microsoft.AspNetCore.Http;

namespace LancamentoFinanceiro.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Service
            services.AddScoped<ILancamentoFinanceiroService, LancamentoFinanceiroService>();
            services.AddScoped<IItemLancamentoFinanceiroService, ItemLancamentoFinanceiroService>();
            services.AddScoped<IRentabilidadeCelulaService, RentabilidadeCelulaService>();
            services.AddScoped<IRentabilidadeDiretoriaService, RentabilidadeDiretoriaService>();
            services.AddScoped<IMigracaoRMService, MigracaoRMService>();
            services.AddScoped<IServicoContratadoService, ServicoContratadoService>();

            //Infra Data
            services.AddScoped<ILancamentoFinanceiroRepository, LancamentoFinanceiroRepository>();
            services.AddScoped<IItemLancamentoFinanceiroRepository, ItemLancamentoFinanceiroRepository>();
            services.AddScoped<ITipoDespesaRepository, TipoDespesaRepository>();
            services.AddScoped<IContratoRepository, ContratoRepository>();
            services.AddScoped<IServicoContratadoRepository, ServicoContratadoRepository>();
            services.AddScoped<ICidadeRepository, CidadeRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddScoped<NotificationHandler>();

            //log
            services.AddScoped<LogGenericoContext>();
            services.AddScoped<ILogGenericoRepository, LogGenericoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            //Globals
            services.AddScoped<Variables>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
