using AutoMapper;

using System.Linq;
using UsuarioApi.Domain.DominioRoot.Dto;
using UsuarioApi.Domain.DominioRoot.Entity;

namespace UsuarioApi.Api.AutoMapper
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<Usuario, UsuarioDto>();
           

        }
    }
}
