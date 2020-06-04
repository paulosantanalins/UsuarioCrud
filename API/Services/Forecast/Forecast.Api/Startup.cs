using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Forecast.Api.Configuration;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Utils;
using Utils.Connections;
using Utils.Base;
using Microsoft.EntityFrameworkCore;
using Forecast.Infra.Data.SqlServer.Context;
using Forecast.Api.Handler;

namespace Forecast.Api
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
            services.AddSingleton(linksBase);
            services.AddSingleton(urls);
            services.AddSingleton(settings);

            Variables.DefaultConnection = settings.DefaultConnection;
            services.AddDbContext<ForecastContext>(builder => builder.UseSqlServer(settings.DefaultConnection));

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<Utils.Middleware.UserNameCustomMiddleware>();
            app.UseCors("AllowAll");
            app.UseMvc();


            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hangfire"))
                {
                    context.Request.PathBase = "/api/forecast";
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
           

            app.UseMvc();
        }
    }
}
