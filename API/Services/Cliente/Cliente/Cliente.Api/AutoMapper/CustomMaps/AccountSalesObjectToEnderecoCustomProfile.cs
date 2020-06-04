using AutoMapper;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Salesforce.Models;

namespace Cliente.Api.AutoMapper.CustomMaps
{
    public class AccountSalesObjectToEnderecoCustomProfile : Profile
    {
        public AccountSalesObjectToEnderecoCustomProfile()
        {
            CreateMap<AccountSalesObject, Endereco>()
                .ForMember(src => src.NrCep, opt => opt.MapFrom(x => x.CNPJ_CEP__c))
                .ForMember(src => src.NmBairro, opt => opt.MapFrom(x => x.CNPJ_Bairro__c))
                .ForMember(src => src.NmCompEndereco, opt => opt.MapFrom(x => x.CNPJ_Complemento__c))
                .ForMember(src => src.NrEndereco, opt => opt.MapFrom(x => x.CNPJ_Endereco_Numero__c))
                .ForMember(src => src.NmEndereco, opt => opt.MapFrom(x => x.CNPJ_Logradouro__c))
                .ForMember(src => src.SgAbrevLogradouro, opt => opt.MapFrom(x => x.TipoDeEndereco__c))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
