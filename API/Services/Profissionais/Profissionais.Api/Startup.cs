using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UsuarioApi.Api.Configuration;
using Swashbuckle.AspNetCore.Swagger;
using Utils;
using Utils.Base;
using Utils.Connections;
using UsuarioApi.Api.Handlers;

namespace UsuarioApi.Api
{
    public class Startup
    {
        private IHostingEnvironment CurrentEnvironment { get; }
        public IConfiguration Configuration { get; }

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
           
            var linksBase = Configuration.GetSection("LinksBase").Get<LinkBase>();
    
            services.AddSingleton(settings);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddDIConfiguration();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API Pública", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builderCors =>
                    {
                        builderCors
                         .WithOrigins(new string[] { "http://localhost:4200", })
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Pública");
            });

            app.UseCookiePolicy();
            app.UseHttpsRedirection();

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new CustomExceptionHandler().Invoke
            });

            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
