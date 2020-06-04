using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.PrestadorRoot.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.AutoMapper
{
    public class ViewModelToDtoMappingProfile : Profile
    {      
        public ViewModelToDtoMappingProfile()
        {
            CreateMap<ExtensaoContratoPrestadorVM, ExtensaoContratoPrestadorDto>();
        }
    }
}
