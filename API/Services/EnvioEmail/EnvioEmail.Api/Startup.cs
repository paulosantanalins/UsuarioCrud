using AutoMapper;
using EnvioEmail.Api.Configuration;
using EnvioEmail.Api.Handler;
using EnvioEmail.Api.Jobs;
using EnvioEmail.Infra.Data.SqlServer.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utils;
using Utils.Connections;
using Hangfire;
using Hangfire.SqlServer;
using System;
using Utils.Base;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace EnvioEmail.Api
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
            Variables.EmailRemetente = Configuration.GetSection("EmailRemetente").Value;
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            var settings = Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();

            var credenciais = Configuration.GetSection("Credenciais").Get<Credenciais>();
            Variables.Host = credenciais.GetHost();
            Variables.Port = credenciais.GetPort();
            Variables.UseDefaultCredentials = credenciais.GetUseDefaultCredentials();
            Variables.EnableSsl = credenciais.GetEnableSsl();

            services.AddSingleton(settings);

            Variables.DefaultConnection = settings.DefaultConnection;
            services.AddDbContext<ServiceBContext>(builder => builder.UseSqlServer(settings.DefaultConnection));
            
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IRecurringJobManager recurringJobManager, IHostingEnvironment env)
        {
            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hangfire"))
                {
                    context.Request.PathBase = "/api/envioemail";
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
                AppPath = "/api/envioemail"
            });
            InstanciarJobsHangFire(recurringJobManager);

            app.UseMvc();
        }

        private void InstanciarJobsHangFire(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<EnviarEmailComErro>("reenviar-email", x => x.Execute(), "0 */2 * * *");
        }
    }
}
