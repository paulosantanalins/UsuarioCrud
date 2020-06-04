using AutoMapper;
using Seguranca.Api.ViewModels;
using Seguranca.Domain.UsuarioRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seguranca.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
           CreateMap<UsuarioVM, Usuario>();
        }
    }
}
