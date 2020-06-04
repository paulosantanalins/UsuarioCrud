using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.Jobs
{
    public class Provider<T> : IProvider<T>
    {
        IHttpContextAccessor contextAccessor;

        public Provider(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        T IProvider<T>.Get()
        {
            return contextAccessor.HttpContext.RequestServices.GetService<T>();
        }
    }
}
