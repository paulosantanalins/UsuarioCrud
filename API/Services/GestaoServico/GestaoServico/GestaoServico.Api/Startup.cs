using AutoMapper;
using FluentScheduler;
using GestaoServico.Api.Configuration;
using GestaoServico.Api.Handler;
using GestaoServico.Api.Jobs.Base;
using GestaoServico.Infra.Data.SqlServer.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utils;
using Utils.Base;
using Utils.Connections;

namespace GestaoServico.Api
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
            services.AddDbContext<GestaoServicoContext>(builder => builder.UseSqlServer(settings.DefaultConnection));

            JobManager.Initialize(new JobRegistry());
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            //services.AddSingleton<IScheduledTask, ObterContratosSalesForceJob>();
            //services.AddSingleton<ISalesForceAuthenticationService, SalesForceAuthenticationService>();
            //services.AddTransient<IProvider<IContratoService>, Provider<IContratoService>>();
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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            app.UseMvc();
        }
    }
}
