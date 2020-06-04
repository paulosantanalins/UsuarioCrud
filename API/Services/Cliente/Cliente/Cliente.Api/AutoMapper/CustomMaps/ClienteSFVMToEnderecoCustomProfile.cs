using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Api.AutoMapper.CustomMaps
{
    public class ClienteSFVMToEnderecoCustomProfile : Profile
    {
        public ClienteSFVMToEnderecoCustomProfile()
        {
            CreateMap<ClienteSFVM, Endereco>()
                .ForMember(src => src.SgAbrevLogradouro, opt => opt.MapFrom(x => x.abrevLogradouro))
                .ForMember(src => src.NmEndereco, opt => opt.MapFrom(x => x.endereco))
                .ForMember(src => src.NrEndereco, opt => opt.MapFrom(x => x.numero))
                .ForMember(src => src.NmCompEndereco, opt => opt.MapFrom(x => x.compEndereco))
                .ForMember(src => src.NmBairro, opt => opt.MapFrom(x => x.bairro))
                .ForMember(src => src.NrCep, opt => opt.MapFrom(x => x.cep));
        }
    }
}
