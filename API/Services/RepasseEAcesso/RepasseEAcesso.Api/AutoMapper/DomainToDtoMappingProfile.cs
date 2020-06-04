using AutoMapper;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Dto;
using RepasseEAcesso.Domain.PeriodoRepasseRoot.Entity;
using RepasseEAcesso.Domain.RepasseRoot.Dto;
using RepasseEAcesso.Domain.RepasseRoot.Entity;
using RepasseEAcesso.Domain.SharedRoot.DTO;
using System;
using System.Globalization;

namespace RepasseEAcesso.Api.AutoMapper
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<PeriodoRepasse, PeriodoRepasseDto>()
                .ForMember(x => x.MesLancamento, opt => opt.MapFrom(x => x.DtLancamento.Month))
                .ForMember(x => x.AnoLancamento, opt => opt.MapFrom(x => x.DtLancamento.Year));
            CreateMap<RepasseNivelUm, RepasseNivelUmDto>();            
            CreateMap<LogRepasse, LogRepasseDto>();

            CreateMap<PeriodoRepasse, ComboDefaultDto>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.DtLancamento.ToString("MMMM/yyyy", CultureInfo.CreateSpecificCulture("pt")).ToUpper()));
        }      
    }
}

