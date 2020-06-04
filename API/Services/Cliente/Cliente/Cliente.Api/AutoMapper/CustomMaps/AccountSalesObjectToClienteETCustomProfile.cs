using AutoMapper;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Salesforce.Models;

namespace Cliente.Api.AutoMapper.CustomMaps
{
    public class AccountSalesObjectToClienteETCustomProfile : Profile
    {
        public AccountSalesObjectToClienteETCustomProfile()
        {
            CreateMap<AccountSalesObject, ClienteET>()
                .ForMember(dest => dest.FlTipoHierarquia, opt => opt.MapFrom(src => src.Type == "Filha" ? "F" : "M"))
                .ForMember(dest => dest.IdSalesforce, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NrInscricaoEstadual, opt => opt.MapFrom(src => src.InscricaoEstadual__c))
                .ForMember(dest => dest.NrInscricaoMunicipal, opt => opt.MapFrom(src => src.InscricaoMunicipal__c))
                .ForMember(dest => dest.NmEmail, opt => opt.MapFrom(src => src.E_MAIL__c))
                .ForMember(dest => dest.NmSite, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.NrFax, opt => opt.MapFrom(src => src.Fax))
                .ForMember(dest => dest.FlStatus, opt => opt.MapFrom(src => src.StatusConta__c == "Ativo" ? "A" : "I"))
                .ForMember(dest => dest.NrTelefone, opt => opt.MapFrom(src => src.Phone.Trim()))
                .ForMember(dest => dest.NrTelefone2, opt => opt.MapFrom(src => src.Phone_2__c.Trim()))
                .ForMember(dest => dest.NmRazaoSocial, opt => opt.MapFrom(src => src.RazaoSocial__c))
                .ForMember(dest => dest.NmFantasia, opt => opt.MapFrom(src => src.NomeFantasia__c != null ? src.NomeFantasia__c : ""))
                .ForMember(dest => dest.NrCnpj, opt => opt.MapFrom(src => src.CNPJ__c))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdCliente));
        }
    }
}
