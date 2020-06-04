using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using Utils.EacessoLegado.Models;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class EmpresaEacessoToEmpresaVMCustomProfile : Profile
    {
        public EmpresaEacessoToEmpresaVMCustomProfile()
        {
            CreateMap<EmpresaEacesso, EmpresaVM>()
                .ForMember(src => src.Id, opt => opt.MapFrom(x => x.IdEmpresa))
                .ForMember(src => src.NmEmpresa, opt => opt.MapFrom(x => x.Empresa));
        }
    }
}
