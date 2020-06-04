using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ServicoIntegracaoVMParaVinculoClienteServicoCustomProfile : Profile
    {
        public ServicoIntegracaoVMParaVinculoClienteServicoCustomProfile()
        {
            CreateMap<ServicoIntegracaoVM, PacoteServico>()
                //Confirmar valores
                .ForMember(src => src.NmClienteServico, opt => opt.MapFrom(x => x.Nome))
                .ForMember(src => src.DescClienteServico, opt => opt.MapFrom(x => x.Descricao))


                .ForMember(src => src.DtFinalizacao, opt => opt.MapFrom(x => x.DtFinalizacao))
                .ForMember(src => src.FlMensal, opt => opt.MapFrom(x => x.Mensal))
                .ForMember(src => src.FlQuater, opt => opt.MapFrom(x => x.FlagQuarter))
                .ForMember(src => src.FlReembolso, opt => opt.MapFrom(x => x.FlagReembolso))
                .ForMember(src => src.HrExtraReembolso, opt => opt.MapFrom(x => x.ExtrasReemb))
                .ForMember(src => src.IdCliente, opt => opt.MapFrom(x => x.IdCliente))
                .ForMember(src => src.IdDelivery, opt => opt.MapFrom(x => x.IdDelivery))
                .ForMember(src => src.IdSalesForce, opt => opt.MapFrom(x => x.IdSalesForce))
                .ForMember(src => src.NmDelivery, opt => opt.MapFrom(x => x.Delivery))
                .ForMember(src => src.NmEscopo, opt => opt.MapFrom(x => x.Escopo))
                .ForMember(src => src.NmLocalServico, opt => opt.MapFrom(x => x.LocalServico))
                .ForMember(src => src.NrCodificacao, opt => opt.MapFrom(x => x.Codificacao))
                .ForMember(src => src.VlHoraExtra, opt => opt.MapFrom(x => x.VHoraExtra))
                .ForMember(src => src.VlMarkup, opt => opt.MapFrom(x => x.Markup))
                .ForMember(src => src.VlRentabilidade, opt => opt.MapFrom(x => x.Rentabilidade));

                //Verificar
                //.ForMember(src => src.VlReembolso, opt => opt.MapFrom(x => x.Ree))
                //.ForMember(src => src.IdServico, opt => opt.MapFrom(x => x.IdSalesForce))
                //.ForMember(src => src.FlHoraExtraPgMensal, opt => opt.MapFrom(x => x.Descricao))

        }
    }
}
