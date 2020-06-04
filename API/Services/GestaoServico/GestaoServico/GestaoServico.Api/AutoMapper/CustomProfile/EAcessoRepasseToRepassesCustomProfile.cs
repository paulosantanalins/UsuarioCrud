using AutoMapper;
using GestaoServico.Api.AutoMapper.Resolvers;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class EAcessoRepasseToRepassesCustomProfile : Profile
    {
        public EAcessoRepasseToRepassesCustomProfile()
        {
            CreateMap<EAcessoRepasse, Repasse>()
                .ForMember(dest => dest.NrParcela, opt => opt.ResolveUsing<CustomResolver>())
                //.ForMember(dest => dest.IdServicoContratadoOrigem, opt => opt.ResolveUsing<ServicoOrigemResolve>())
                //.ForMember(dest => dest.IdServicoContratadoDestino, opt => opt.ResolveUsing<ServicoDestinoEacessoResolver>())
                .ForMember(dest => dest.VlInc, opt => opt.MapFrom(x => x.VlInc ?? 0))
                .ForMember(dest => dest.VlDesc, opt => opt.MapFrom(x => x.VlDesconto ?? 0))
                .ForMember(dest => dest.IdRepasseEacesso, opt => opt.MapFrom(x => x.IdEpm))
                .ForMember(dest => dest.Usuario, opt => opt.MapFrom(x => "EACESSO"))
                .ForMember(dest => dest.IdRepasseEacesso, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => 0))
                .ForMember(dest => dest.DataAlteracao, opt => opt.MapFrom(x => DateTime.Now.Date));
        }
    }
}
