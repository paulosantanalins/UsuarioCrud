using AutoMapper;
using Cliente.Api.ViewModels;
using Cliente.Domain.ClienteRoot.Dto;
using Utils;

namespace Cliente.Api.AutoMapper
{
    public class ViewModelToDtoMappingProfile : Profile
    {
        public ViewModelToDtoMappingProfile()
        {
            CreateMap<ClienteVM,ClienteEacessoDto>();
            CreateMap<ContatoClienteEacessoVM, ContatoClienteEacessoDto>();
            CreateMap<ClienteLocalTrabalhoEacessoVM, ClienteLocalTrabalhoEacessoDto>();
            CreateMap<FiltroGenericoViewModel<ClienteVM>,FiltroGenericoDto<ClienteEacessoDto>>();
        }
    }
}
