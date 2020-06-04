using AutoMapper;
using LancamentoFinanceiro.Api.ViewModels.Rentabilidade;
using LancamentoFinanceiro.Domain.RentabilidadeRoot.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LancamentoFinanceiro.Api.AutoMapper
{
    public class ViewModelToDtoMappingProfile : Profile
    {
        public ViewModelToDtoMappingProfile()
        {
            DateTime? data = null;
            CreateMap<FiltroRelatorioRentabilidadeCelulaVM, FiltroRelatorioRentabilidadeCelulaDto>()
                .ForMember(src => src.DtInicio, opt => opt.MapFrom(x => x.DtFim != null ? new DateTime(Int32.Parse(x.DtInicio.Substring(0, 4)), Int32.Parse(x.DtInicio.Substring(5, 2)), Int32.Parse(x.DtInicio.Substring(8, 2))) : data))
                .ForMember(src => src.DtFim, opt => opt.MapFrom(x => x.DtFim != null ? new DateTime(Int32.Parse(x.DtFim.Substring(0, 4)), Int32.Parse(x.DtFim.Substring(5, 2)), Int32.Parse(x.DtFim.Substring(8, 2))) : data))
                .ReverseMap();

            CreateMap<FiltroRelatorioRentabilidadeDiretoriaVM, FiltroRelatorioRentabilidadeDiretoriaDto>()
               .ForMember(src => src.DtInicio, opt => opt.MapFrom(x => x.DtFim != null ? new DateTime(Int32.Parse(x.DtInicio.Substring(0, 4)), Int32.Parse(x.DtInicio.Substring(5, 2)), Int32.Parse(x.DtInicio.Substring(8, 2))) : data))
               .ForMember(src => src.DtFim, opt => opt.MapFrom(x => x.DtFim != null ? new DateTime(Int32.Parse(x.DtFim.Substring(0, 4)), Int32.Parse(x.DtFim.Substring(5, 2)), Int32.Parse(x.DtFim.Substring(8, 2))) : data))
               .ReverseMap();
        }
    }
}
