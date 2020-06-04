using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Api.ViewModels.ControleAcesso;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;

namespace ControleAcesso.Api.AutoMapper
{
    public class ViewModelToDtoMapping : Profile
    {
        public ViewModelToDtoMapping()
        {
            CreateMap<PerfilVM, PerfilDto>().ReverseMap();
            CreateMap<UsuarioPerfilVM, UsuarioPerfilDto>().ReverseMap();
            CreateMap<FiltroCelulasAdVM, UsuarioPerfilDto>().ReverseMap();

            CreateMap<UsuarioSegurancaDto, UsuarioVisualizacaoCelulaDto>()
                .ForMember(x => x.LgUsuario, opt => opt.MapFrom(x => x.Login));
        }
    }
}
