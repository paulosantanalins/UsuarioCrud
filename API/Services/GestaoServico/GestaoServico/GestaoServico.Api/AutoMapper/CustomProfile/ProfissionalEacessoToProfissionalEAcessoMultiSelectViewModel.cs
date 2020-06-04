using AutoMapper;
using GestaoServico.Api.ViewModels;
using Utils.Base;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ProfissionalEacessoToProfissionalEAcessoMultiSelectViewModel : Profile
    {
        public ProfissionalEacessoToProfissionalEAcessoMultiSelectViewModel()
        {
            CreateMap<ProfissionalEacesso, ProfissionalEAcessoMultiSelectVM>()
                .ForMember(dest => dest.IdSecundario, opt => opt.MapFrom(src => src.Situacao));
        }
    }
}
