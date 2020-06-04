using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.AutoMapper.CustomMaps
{
    public class ClienteSFVMToClienteETCustomProfile : Profile
    {
        public ClienteSFVMToClienteETCustomProfile()
        {
            CreateMap<ClienteSFVM, ClienteET>()
                .ForMember(src => src.NrCnpj, opt => opt.MapFrom(x => x.cnpj))
                .ForMember(src => src.NmFantasia, opt => opt.MapFrom(x => x.nomeFantasia))
                .ForMember(src => src.NmRazaoSocial, opt => opt.MapFrom(x => x.razaoSocial))
                .ForMember(src => src.IdSalesforce, opt => opt.MapFrom(x => x.idSalesForce));
        }
    }
}
