using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Dto;
using Cliente.Domain.SharedRoot;
using Utils;
using Utils.Extensions;
namespace Cliente.Api.AutoMapper
{
    public class DtoToViewModelMappingProfile : Profile
    {
        public DtoToViewModelMappingProfile()
        {
            CreateMap<ClienteEacessoDto, ClienteVM>()
                .ForMember(x => x.TipoClienteRM, opt => opt.MapFrom(x => ((SharedEnuns.TipoClienteRM)x.TipoClienteRM).GetDescription()));
            CreateMap<ContatoClienteEacessoDto, ContatoClienteEacessoVM>();
            CreateMap<TelefoneContatoClienteDto, TelefoneContatoClienteVM>();
            CreateMap<ClienteLocalTrabalhoEacessoDto, ClienteLocalTrabalhoEacessoVM>();
            CreateMap<FiltroGenericoViewModel<ClienteEacessoDto>, FiltroGenericoDto<ClienteVM>>().ReverseMap();
        }
    }
}
