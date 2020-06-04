using AutoMapper;
using Cliente.Domain.ClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;
using Utils.Salesforce.Models;

namespace Cliente.Api.AutoMapper.CustomMaps
{
    public class ClienteEacessoToAccountSalesObjectCustomProfile : Profile
    {
        public ClienteEacessoToAccountSalesObjectCustomProfile()
        {
            CreateMap<ClienteEacesso, AccountSalesObject>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SalesForceID))
                .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.SiglaSetorEconomico))
                .ForMember(dest => dest.Ramo__c, opt => opt.MapFrom(src => src.SiglaRamoAtividade))
                .ForMember(dest => dest.RazaoSocial__c, opt => opt.MapFrom(src => src.RazaoSocial))
                .ForMember(dest => dest.NomeFantasia__c, opt => opt.MapFrom(src => src.NomeFantasia))
                .ForMember(dest => dest.CNPJ__c, opt => opt.MapFrom(src => src.CNPJ))


                //Endereco
                .ForMember(src => src.CNPJ_CEP__c, opt => opt.MapFrom(x => x.Cep))
                .ForMember(src => src.CNPJ_Bairro__c, opt => opt.MapFrom(x => x.Bairro))
                .ForMember(src => src.CNPJ_Complemento__c, opt => opt.MapFrom(x => x.CompEndereco))
                .ForMember(src => src.CNPJ_Endereco_Numero__c, opt => opt.MapFrom(x => x.Numero))
                .ForMember(src => src.CNPJ_Logradouro__c, opt => opt.MapFrom(x => x.Endereco))
                .ForMember(src => src.TipoDeEndereco__c, opt => opt.MapFrom(x => x.AbrevLogradouro))
                .ForMember(src => src.CNPJ_Cidade__c, opt => opt.MapFrom(x => x.Cidade))
                .ForMember(src => src.CNPJ_Estado__c, opt => opt.MapFrom(x => x.SiglaEstado))
                .ForMember(src => src.PaisConta__c, opt => opt.MapFrom(x => x.SiglaPais))


                //valores mantidos
                .ForMember(dest => dest.Type, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Website, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.StatusConta__c, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Phone_2__c, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.ParentId, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Name, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.IdGrupoCliente, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Fax, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.E_MAIL__c, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.Description, opt => opt.UseDestinationValue())

                //tirar duvida
                .ForMember(dest => dest.InscricaoMunicipal__c, opt => opt.MapFrom(src => src.InscrMunicipal))
                .ForMember(dest => dest.InscricaoEstadual__c, opt => opt.MapFrom(src => src.InscrEstadual))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Telefone)); 
                //.ForMember(dest => dest.NmEmail, opt => opt.MapFrom(src => src.E_MAIL__c))

                //nao tem
                //.ForMember(dest => dest.NmSite, opt => opt.MapFrom(src => src.Website))
                //.ForMember(dest => dest.NrFax, opt => opt.MapFrom(src => src.Fax))
                //.ForMember(dest => dest.FlStatus, opt => opt.MapFrom(src => src.StatusConta__c == "Ativo" ? "A" : "I"))
                //.ForMember(dest => dest.NrTelefone2, opt => opt.MapFrom(src => src.Phone_2__c))
        }
    }
}
