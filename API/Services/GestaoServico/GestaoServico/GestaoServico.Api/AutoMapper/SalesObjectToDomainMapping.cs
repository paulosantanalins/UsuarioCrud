using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Salesforce.Models;

namespace GestaoServico.Api.AutoMapper
{
    public class SalesObjectToDomainMapping : Profile
    {
        public SalesObjectToDomainMapping()
        {
            CreateMap<ContratoSalesObject, Contrato>()
                .ForMember(src => src.NrAssetSalesForce, opt => opt.MapFrom(x => x.ContractNumber))
                .ForMember(src => src.DescContrato, opt => opt.MapFrom(x => x.N_mero_contrato_interno__c))
                .ForMember(src => src.DescStatusSalesForce, opt => opt.MapFrom(x => x.Status))
                .ForMember(src => src.DtFinalizacao, opt => opt.MapFrom(x => x.EndDate))
                .ForMember(src => src.DtInicial, opt => opt.MapFrom(x => x.StartDate));
        }
    }
}
