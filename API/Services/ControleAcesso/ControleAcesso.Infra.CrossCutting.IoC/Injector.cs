using Microsoft.Extensions.DependencyInjection;
using ControleAcesso.Domain.Core.Notifications;
using System;
using ControleAcesso.Domain.Interfaces;
using Utils;
using ControleAcesso.Infra.Data.SqlServer.UoW;
using ControleAcesso.Domain.ControleAcessoRoot.Repository;
using ControleAcesso.Infra.Data.SqlServer.Repository.ControleAcesso;
using ControleAcesso.Domain.ControleAcessoRoot.Service.Interfaces;
using ControleAcesso.Domain.ControleAcessoRoot.Service;
using Logger.Repository.Interfaces;
using Logger.Context;
using Logger.Repository;
using Microsoft.AspNetCore.Http;
using ControleAcesso.Domain.DominioRoot.Repository;
using ControleAcesso.Domain.DominioRoot.Service;
using ControleAcesso.Domain.DominioRoot.Service.Interfaces;
using GestaoServico.Domain.PessoaRoot.Service.Interfaces;
using GestaoServico.Domain.PessoaRoot.Service;
using GestaoServico.Domain.PessoaRoot.Repository;
using GestaoServico.Infra.Data.SqlServer.Repository.PessoaRepository;
using ControleAcesso.Domain.BroadcastRoot.Service.Interfaces;
using ControleAcesso.Domain.BroadcastRoot.Service;
using ControleAcesso.Domain.BroadcastRoot.Repository;
using ControleAcesso.Domain.MonitoramentoRoot.Repository;
using ControleAcesso.Domain.MonitoramentoRoot.Service.Interfaces;
using ControleAcesso.Domain.MonitoramentoRoot.Service;

namespace ControleAcesso.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Service
            services.AddScoped<IFuncionalidadeService, FuncionalidadeService>();
            services.AddScoped<IPerfilService, PerfilService>();
            services.AddScoped<IUsuarioPerfilService, UsuarioPerfilService>();
            services.AddScoped<IVisualizacaoCelulaService, VisualizacaoCelulaService>();
            services.AddScoped<ICelulaService, CelulaService>();
            services.AddScoped<IDominioService, DominioService>();
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IBroadcastService, BroadcastService>();
            services.AddScoped<IBroadcastItemService, BroadcastItemService>();
            services.AddScoped<IMonitoramentoBackService, MonitoramentoBackService>();

            //Infra Data
            services.AddScoped<IFuncionalidadeRepository, FuncionalidadeRepository>();
            services.AddScoped<IPerfilRepository, PerfilRepository>();
            services.AddScoped<IUsuarioPerfilRepository, UsuarioPerfilRepository>();
            services.AddScoped<IVinculoPerfilFuncionalidadeRepository, VinculoPerfilFuncionalidadeRepository>();
            services.AddScoped<IVinculoTipoCelulaTipoContabilRepository, VinculoTipoCelulaTipoContabilRepository>();
            services.AddScoped<IVisualizacaoCelulaRepository, VisualizacaoCelulaRepository>();
            services.AddScoped<ICelulaRepository, CelulaRepository>();
            services.AddScoped<IDominioRepository, DominioRepository>();
            services.AddScoped<ITipoCelulaRepository, TipoCelulaRepository>();
            services.AddScoped<IGrupoRepository, GrupoRepository>();
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IBroadcastRepository, BroadcastRepository>();
            services.AddScoped<IBroadcastItemRepository, BroadcastItemRepository>();
            services.AddScoped<IMonitoramentoBackRepository, MonitoramentoBackRepository>();

            //log
            services.AddScoped<LogGenericoContext>();
            services.AddScoped<ILogGenericoRepository, LogGenericoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IPessoaRepository, PessoaRepository>();

            //Globals
            services.AddScoped<Variables>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<NotificationHandler>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
