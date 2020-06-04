using AutoMapper;
using Cadastro.Api.Configuration;
using Cadastro.Api.Handler;
using Cadastro.Api.Jobs;
using Cadastro.Infra.Data.SqlServer.Context;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using Cadastro.Domain.SharedRoot;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace Cadastro.Api
{
    public class Startup
    {
        private IHostingEnvironment CurrentEnvironment { get; }
        private IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            CurrentEnvironment = env;
            Variables.EnvironmentName = CurrentEnvironment.EnvironmentName;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            var settings = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
            var urls = Configuration.GetSection("APIs").Get<MicroServicosUrls>();
            var linksBase = Configuration.GetSection("LinksBase").Get<LinkBase>();
            services.AddSingleton(urls);
            services.AddSingleton(linksBase);
            services.AddSingleton(settings);

            Variables.DefaultConnection = settings.DefaultConnection;
            services.AddDbContext<CadastroContexto>(builder => builder.UseSqlServer(settings.DefaultConnection));

            Variables.EacessoConnection = settings.EacessoConnection;
            services.AddDbContext<CadastroContexto>(builder => builder.UseSqlServer(settings.EacessoConnection));

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddAutoMapper();
            services.AddDIConfiguration();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builderCors =>
                    {
                        builderCors
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer();

            var ldaps = Configuration.GetSection("ldaps").Get<LdapSeguranca[]>();
            services.AddSingleton(ldaps);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IRecurringJobManager recurringJobManager, IHostingEnvironment env)
        {
            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hangfire"))
                {
                    context.Request.PathBase = "/api/cadastro";
                }
                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = new CustomExceptionHandler().Invoke
                });
            }

            app.UseMiddleware<Utils.Middleware.UserNameCustomMiddleware>();
            app.UseCors("AllowAll");


            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() },
                AppPath = "/api/cadastro"
            });
            InstanciarJobsHangFire(recurringJobManager);

            app.UseMvc();
        }

        private void InstanciarJobsHangFire(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<EnviarEmailParaEnvioNfJob>("solicitar-envio-nf-prestador", x => x.Execute(), "20 11 * * *");
            recurringJobManager.AddOrUpdate<AtualizarImportacaoPagamentoRMJob>("atualizar-status-pagamento-prestador", x => x.Execute(), "00 */2 * * *");
            recurringJobManager.AddOrUpdate<EnviarEmailParaAprovacaoHorasJob>("solicitar-aprovacao-horas-prestador", x => x.Execute(), "00 11 * * *");
            recurringJobManager.AddOrUpdate<AtualizarInformacaoCelulaJob>("atualizar-informacao-celula", x => x.Execute(), "00 9 * * *");
            recurringJobManager.AddOrUpdate<AtualizarMigracaoPessoaJob>("atualizar-migracao-pessoa", x => x.Execute(), "00 9 * * *");
            recurringJobManager.AddOrUpdate<RealizarTransferenciaPrestadorJob>("efetivar-transferencias-prestadores", x => x.Execute(), "00 2 * * *");
            recurringJobManager.AddOrUpdate<RealizarFinalizacaoContratosJob>("efetivar-finalizacao-contratos", x => x.Execute(), "00 2 * * *");
            recurringJobManager.AddOrUpdate<MigrarProfissionaisNatcorpJob>("efetivar-finalizacao-contratos", x => x.Execute(), "00 */1 * * *");
            recurringJobManager.AddOrUpdate<RealizarReajusteContratosJob>("Efetivar-reajuste-contratos", x => x.Execute(), "00 2 * * *");
            //TO_DO testar
            recurringJobManager.AddOrUpdate<MigrarTodosProfissionaisNatcorpJob>("efetivar-finalizacao-contratos", x => x.Execute(), "00 2 * * *");
        }
    }
}
