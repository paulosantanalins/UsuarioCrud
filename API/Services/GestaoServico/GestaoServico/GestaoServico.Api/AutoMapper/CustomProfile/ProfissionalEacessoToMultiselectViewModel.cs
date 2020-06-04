using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ProfissionalEacessoToMultiselectViewModel : Profile
    {
        public ProfissionalEacessoToMultiselectViewModel()
        {
            CreateMap<ProfissionalEacesso, MultiselectViewModel>()
                .ForMember(dest => dest.IdSecundario, opt => opt.MapFrom(src => src.Situacao));
        }
    }
}
