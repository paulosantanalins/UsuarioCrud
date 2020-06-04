using AutoMapper;
using UsuarioApi.Domain.DominioRoot.Dto;
using UsuarioApi.Domain.DominioRoot.Entity;

namespace UsuarioApi.Api.AutoMapper
{
    public class DtoToDomainMappingProfile : Profile
    {
        public DtoToDomainMappingProfile()
        {

            CreateMap<UsuarioDto, Usuario>();

        }
    }
}
