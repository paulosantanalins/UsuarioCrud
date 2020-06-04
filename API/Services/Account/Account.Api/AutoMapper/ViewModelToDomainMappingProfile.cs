using AutoMapper;
using Account.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account.Domain.ProjetoRoot.Entity;

namespace Account.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
           CreateMap<ProjetoViewModel, Sistema>();
        }
    }
}
