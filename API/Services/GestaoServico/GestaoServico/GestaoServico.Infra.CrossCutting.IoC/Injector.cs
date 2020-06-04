using GestaoServico.Domain.Core.Notifications;
using GestaoServico.Domain.GestaoFilialRoot.Repository;
using GestaoServico.Domain.GestaoFilialRoot.Service;
using GestaoServico.Domain.GestaoFilialRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Repository;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoPortifolioRoot.Repository;
using GestaoServico.Domain.GestaoPortifolioRoot.Service;
using GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Repository;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Service.Interfaces;
using GestaoServico.Domain.Interfaces;
using GestaoServico.Domain.OperacaoMigradaRoot.Repository;
using GestaoServico.Domain.OperacaoMigradaRoot.Service;
using GestaoServico.Domain.OperacaoMigradaRoot.Service.Interfaces;
using GestaoServico.Domain.PessoaRoot.Repository;
using GestaoServico.Domain.PessoaRoot.Service;
using GestaoServico.Domain.PessoaRoot.Service.Interfaces;
using GestaoServico.Infra.Data.SqlServer.Context;
using GestaoServico.Infra.Data.SqlServer.Repository.GestaoPacoteServico;
using GestaoServico.Infra.Data.SqlServer.Repository.GestaoPortfolioRepository;
using GestaoServico.Infra.Data.SqlServer.Repository.GestaoServicoContratadoServico;
using GestaoServico.Infra.Data.SqlServer.Repository.PessoaRepository;
using GestaoServico.Infra.Data.SqlServer.UoW;
using Logger.Context;
using Logger.Repository;
using Logger.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using Utils;

namespace GestaoServico.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Domain Service
            services.AddScoped<ICategoriaContabilService, CategoriaContabilService>();
            services.AddScoped<IClassificacaoContabilService, ClassificacaoContabilService>();            
            //services.AddScoped<ITipoServicoService, TipoServicoService>();
            services.AddScoped<IPortfolioServicoService, PortfolioServicoService>();
            services.AddScoped<IContratoService, ContratoService>();
            services.AddScoped<IServicoContratadoService, ServicoContratadoService>();
            services.AddScoped<ICelulaService, CelulaService>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IRepasseService, RepasseService>();
            services.AddScoped<IEscopoServicoService, EscopoServicoService>();
            services.AddScoped<IDeParaServicoService, DeParaServicoService>();
            services.AddScoped<IRepasseMigracaoService, RepasseMigracaoService>();
            services.AddScoped<IVinculoServicoCelulaComercialService, VinculoServicoCelulaComercialService>();
            services.AddScoped<IOperacaoMigradaService, OperacaoMigradaService>();
            services.AddScoped<IPessoaService, PessoaService>();
            services.AddScoped<IServicoEAcessoService, ServicoEAcessoService>();

            //Infra Data
            services.AddScoped<NotificationHandler>();
            services.AddScoped<IClassificacaoContabilRepository, ClassificacaoContabilRepository>();
            services.AddScoped<ICategoriaContabilRepository, CategoriaContabilRepository>();           
            //services.AddScoped<ITipoServicoRepository, TipoServicoRepository>();
            services.AddScoped<IPortfolioServicoRepository, PortfolioServicoRepository>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<IRepasseRepository, RepasseRepository>();
            services.AddScoped<IEscopoServicoRepository, EscopoServicoRepository>();
            services.AddScoped<IContratoRepository, ContratoRepository>();
            services.AddScoped<IServicoContratadoRepository, ServicoContratadoRepository>();
            services.AddScoped<ICelulaRepository, CelulaRepository>();
            services.AddScoped<IDeParaServicoRepository, DeParaServicoRepository>();
            services.AddScoped<IVinculoMarkupServicoContratadoRepository, VinculoMarkupServicoContratadoRepository>();
            services.AddScoped<IVinculoServicoCelulaComercialRepository, VinculoServicoCelulaComercialRepository>();
            services.AddScoped<IOperacaoMigradaRepository, OperacaoMigradaRepository>();
            services.AddScoped<IPessoaRepository, PessoaRepository>();

            //log
            services.AddScoped<LogGenericoContext>();
            services.AddScoped<ILogGenericoRepository, LogGenericoRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

            //Globals
            services.AddScoped<Variables>();

            services.AddScoped<IVariablesToken, VariablesToken>();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<GestaoServicoContext>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
