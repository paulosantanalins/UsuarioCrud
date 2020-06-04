using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Seguranca.Domain.Core.Notifications;
using Seguranca.Domain.UsuarioRoot;
using Seguranca.Domain.UsuarioRoot.Service;
using Seguranca.Domain.UsuarioRoot.Service.Interfaces;
using Seguranca.Infra.CrossCutting.IoC.JWT;
using Seguranca.Infra.Data.SqlServer.Context;
using System;

namespace Seguranca.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //Domain Service
           // services.AddScoped<IUsuarioService, UsuarioService>();

            //Infra Data
            services.AddScoped<IdentityContext>();
            services.AddScoped<NotificationHandler>();
            services.AddScoped<IJwtFactory, JwtFactory>();
        }
    }
}
