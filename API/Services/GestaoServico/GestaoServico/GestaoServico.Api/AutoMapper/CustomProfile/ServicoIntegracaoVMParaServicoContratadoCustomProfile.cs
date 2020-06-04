using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ServicoIntegracaoVMParaServicoContratadoCustomProfile : Profile
    {
        public ServicoIntegracaoVMParaServicoContratadoCustomProfile()
        {
            CreateMap<ServicoIntegracaoVM, ServicoContratado>()
                .ForMember(src => src.FlReembolso, opt => opt.MapFrom(x => x.FlagReembolso))
                .ForMember(src => src.QtdExtraReembolso, opt => opt.MapFrom(x => x.ExtrasReemb))
                .ForMember(src => src.VlKM, opt => opt.MapFrom(x => x.ValorKM))
                //.ForMember(src => src.VlMarkup, opt => opt.MapFrom(x => x.Markup))
                .ForMember(src => src.VlRentabilidade, opt => opt.MapFrom(x => x.Rentabilidade))
                .ForMember(src => src.Contrato, opt => opt.Ignore());

            //verificar
            //.ForMember(src => src.NmTipoReembolso, opt => opt.MapFrom(x => x.ValorKM))
            //.ForMember(src => src.DtFinal, opt => opt.MapFrom(x => x.Rentabilidade))
            //.ForMember(src => src.DtInicial, opt => opt.MapFrom(x => x.Rentabilidade))
        }
    }
}
