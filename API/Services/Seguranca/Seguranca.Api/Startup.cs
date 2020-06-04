using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Seguranca.Api.Configurations;
using Seguranca.Api.Swagger;
using Seguranca.Api.ViewModels;
using Seguranca.Domain.Core.Notifications;
using Seguranca.Domain.UsuarioRoot;
using Seguranca.Infra.CrossCutting.IoC;
using Seguranca.Infra.CrossCutting.IoC.JWT;
using Seguranca.Infra.CrossCutting.IoC.JwtUtils;
using Seguranca.Infra.Data.SqlServer.Context;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utils;
using Utils.Base;

namespace Seguranca.Api
{
    public class Startup
    {
        private IHostingEnvironment CurrentEnvironment { get; }
        private IConfigurationRoot Configuration { get; }
        private const string SecretKey = "NogameNolife1234567890"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

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

            var aplicacoes = Configuration.GetSection("Aplicacoes").Get<List<AplicacaoVM>>();
            services.AddSingleton(aplicacoes);

            // Adicionado Autenticacao IIS
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            // Add framework services.
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Seguranca")));

            var urls = Configuration.GetSection("APIs").Get<MicroServicosUrls>();
            services.AddSingleton(urls);

            services.AddScoped<NotificationHandler>();
            services.AddScoped<IJwtFactory, JwtFactory>();

            // Configure versions 
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(2, 0);
            });

            // Configure swagger
            services.AddSwaggerGen(options =>
            {
                // Specify two versions 
                options.SwaggerDoc("v1",
                    new Info()
                    {
                        Version = "v1",
                        Title = "micro serviço responsável pela comunição com o AD",
                        Description = "Descrição da API",
                    });

                options.SwaggerDoc("v2",
                    new Info()
                    {
                        Version = "v2",
                        Title = "micro serviço responsável pela comunição com o AD",
                        Description = "Descrição da API",
                       
                    });

                // This call remove version from parameter, without it we will have version as parameter 
                // for all endpoints in swagger UI
                options.OperationFilter<ApiVersionOperationFilter>();

                // This make replacement of v{version:apiVersion} to real version of corresponding swagger doc.
                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                // This on used to exclude endpoint mapped to not specified in swagger version.
                // In this particular example we exclude 'GET /api/v2/Values/otherget/three' endpoint,
                // because it was mapped to v3 with attribute: MapToApiVersion("3")
                options.DocInclusionPredicate((version, desc) =>
                {
                    var versions = desc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var maps = desc.ActionAttributes()
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToArray();

                    return versions.Any(v => $"v{v.ToString()}" == version) && (maps.Length == 0 || maps.Any(v => $"v{v.ToString()}" == version));
                });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Seguranca.Api.xml");
                options.IncludeXmlComments(xmlPath);
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info
            //    {
            //        Version = "v1",
            //        Title = "My first API",
            //        Description = "My First ASP.NET Core 2.0 Web API",
            //        TermsOfService = "None",
            //        Contact = new Contact() { Name = "Neel Bhatt", Email = "neel.bhatt40@gmail.com", Url = "https://neelbhatt40.wordpress.com/" }
            //    });

            //    c.SwaggerDoc("v2", new Info
            //    {
            //        Version = "v2",
            //        Title = "My second API",
            //        Description = "My Second ASP.NET Core 2.0 Web API",
            //        TermsOfService = "None",
            //        Contact = new Contact() { Name = "Neel Bhatt 2", Email = "neel.bhatt40@gmail.com", Url = "https://neelbhatt40.wordpress.com/" }
            //    });

            //    c.DocInclusionPredicate((docName, apiDesc) =>
            //    {
            //        var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
            //        // would mean this action is unversioned and should be included everywhere
            //        if (actionApiVersionModel == null)
            //        {
            //            return true;
            //        }
            //        if (actionApiVersionModel.DeclaredApiVersions.Any())
            //        {
            //            return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
            //        }
            //        return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
            //    });

            //    var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            //    var xmlPath = Path.Combine(basePath, "NeelSwaggerSampleApplication.xml");
            //    c.IncludeXmlComments(xmlPath);
            //    c.OperationFilter<ApiVersionOperationFilter>();
            //});


            services.AddSingleton<IConfiguration>(Configuration);

            // Register the ConfigurationBuilder instance of FacebookAuthSettings

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //TESTE DE LDAP
            //var teste = this.Configuration.GetSection("ldap");
            //services.Configure<Justin.AspNetCore.LdapAuthentication.LdapAuthenticationOptions>(op => 
            //{
            //    op.Hostname = "stefanini.dom";
            //    op.Port = 389;
            //});


            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection("ConnectionStrings");
            //services.Configure<LdapSeguranca>(Configuration.GetSection("ldap"));
            var ldaps = Configuration.GetSection("ldaps").Get<LdapSeguranca[]>();
            services.AddSingleton(ldaps);

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = Configuration["JwtIssuerOptions:Issuer"];
                options.Audience = Configuration["JwtIssuerOptions:Audience"];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer =  Configuration["JwtIssuerOptions:Issuer"],

                ValidateAudience = true,
                ValidAudience = Configuration["JwtIssuerOptions:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Claims.Strings.JwtClaimIdentifiers.Rol, Claims.Strings.JwtClaims.ApiAccess));
                options.AddPolicy("Codigo", policy => policy.RequireClaim(Claims.Strings.JwtClaimIdentifiers.Codigo));
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            // add identity
            var builder = services.AddIdentityCore<Usuario>(o =>
            {
                // configuracao de senha do identity
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;

            });
            //.AddUserManager<Justin.AspNetCore.LdapAuthentication.LdapUserManager<Usuario>>();
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            services.AddAutoMapper();
            services.AddMvc();

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

            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                if (env.IsDevelopment())
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
                else
                    c.SwaggerEndpoint("/api/seguranca/swagger/v2/swagger.json", "My API V2");
            });
        
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
