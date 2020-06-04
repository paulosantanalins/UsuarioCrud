using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UsuarioApi.Domain.SharedRoot.UoW;
using UsuarioApi.Infra.Data.SqlServer.Context;
using UsuarioApi.Infra.Data.SqlServer.Repository;
using UsuarioApi.Domain.DominioRoot.Repository;
using UsuarioApi.Domain.DominioRoot.Service.Interfaces;
using UsuarioApi.Domain.DominioRoot.Service;
using UsuarioApi.Domain.SharedRoot.UoW.Interfaces;
using UsuarioApi.Domain.SharedRoot.Service;
using UsuarioApi.Domain.SharedRoot.Service.Interface;
using UsuarioApi.Domain.SharedRoot.Repository;

namespace UsuarioApi.Infra.CrossCutting.IoC
{
    public class Injector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Infra Data
            services.AddScoped<AppDbContext>();           

            //Services
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            services.AddScoped<IUsuarioService, UsuarioService>();
            

            //Repository
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
