using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LancamentoFinanceiro.Api.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Utils.Connections;
using FluentScheduler;
using LancamentoFinanceiro.Api.Job.Base;
using Utils.Base;
using Utils;
using Microsoft.EntityFrameworkCore;
using LancamentoFinanceiro.Infra.Data.SqlServer.Context;
using LancamentoFinanceiro.Api.Handler;

namespace LancamentoFinanceiro.Api
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
            services.AddAutoMapper();
            services.AddMapper();
            var urls = Configuration.GetSection("APIs").Get<MicroServicosUrls>();
            services.AddSingleton(urls);
            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            
            services.AddDIConfiguration();

            JobManager.Initialize(new JobRegistry());
            services.AddSingleton(settings);

            Variables.DefaultConnection = settings.DefaultConnection;
            services.AddDbContext<ServiceContext>(builder => builder.UseSqlServer(settings.DefaultConnection));

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
