using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ControleAcesso.Api.Configuration;
using Microsoft.AspNetCore.Http;
using Utils.Base;
using Utils.Connections;
using Utils;
using ControleAcesso.Infra.Data.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using ControleAcesso.Api.Handler;
using ControleAcesso.Api.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ControleAcesso.Api
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
            services.AddSingleton(urls);
            services.AddSingleton(settings);
            services.AddAutoMapper();
            services.AddDIConfiguration();
            Variables.DefaultConnection = settings.DefaultConnection;
            services.AddDbContext<ControleAcessoContext>(builder => builder.UseSqlServer(settings.DefaultConnection));
            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IRecurringJobManager recurringJobManager, IHostingEnvironment env)
        {
            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hangfire"))
                {
                    context.Request.PathBase = "/api/controleAcesso";
                }
                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else {
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
                AppPath = "/api/controleAcesso"
            });
            InstanciarJobsHangFire(recurringJobManager);

            app.UseMvc();
        }

        private void InstanciarJobsHangFire(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<FinalizarInativacaoCelulasJob>("efetivar-inativacao-celulas", x => x.Execute(), "00 2 * * *");
        }
    }
}
