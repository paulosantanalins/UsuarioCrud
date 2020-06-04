using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ServicoContratadoToMultiselectViewModelCustomProfile : Profile
    {
        public ServicoContratadoToMultiselectViewModelCustomProfile()
        {
            CreateMap<ServicoContratado, MultiselectViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.EscopoServico.NmEscopoServico))
                .ForMember(dest => dest.IdSecundario, opt => opt.MapFrom(src => src.Contrato != null ? src.Contrato.IdMoeda : 0));
        }
    }
}
