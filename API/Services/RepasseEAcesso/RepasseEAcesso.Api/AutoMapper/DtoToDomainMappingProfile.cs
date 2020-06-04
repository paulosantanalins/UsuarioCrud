using AutoMapper;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;

namespace RepasseEAcesso.Api.AutoMapper
{
    public class DtoToDomainMappingProfile : Profile
    {
        public DtoToDomainMappingProfile()
        {
            CreateMap<PeriodoRepasseDto, PeriodoRepasse>();
            CreateMap<RepasseNivelUmDto, RepasseNivelUm>();
            CreateMap<RepasseNivelUmDto, Repasse>();
            CreateMap<LogRepasseDto, LogRepasse>();
        }
    }
}
