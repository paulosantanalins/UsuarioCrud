using AutoMapper;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;

namespace ControleAcesso.Api.AutoMapper
{
    public class DtoToDomainMappingProfile : Profile
    {
        public DtoToDomainMappingProfile()
        {
            CreateMap<CelulaDto, Celula>()
               .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id));

        }
    }
}
