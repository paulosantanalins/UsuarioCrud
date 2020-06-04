using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoRespasse;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class FiltroAprovarRepasseVMToFiltroAprovarRepasseDTOCustomProfile : Profile
    {
        public FiltroAprovarRepasseVMToFiltroAprovarRepasseDTOCustomProfile()
        {
            DateTime? data = null; 

            CreateMap<FiltroAprovarRepasseVM, FiltroAprovarRepasseDto>()
                .ForMember(src => src.DtInicial, opt => opt.MapFrom(x => x.DtInicial != null ? new DateTime(Int32.Parse(x.DtInicial.Substring(0,4)), Int32.Parse(x.DtInicial.Substring(5, 2)), Int32.Parse(x.DtInicial.Substring(8, 2))) : data))
                .ForMember(src => src.DtFinal, opt => opt.MapFrom(x => x.DtFinal != null ? new DateTime(Int32.Parse(x.DtFinal.Substring(0, 4)), Int32.Parse(x.DtFinal.Substring(5, 2)), Int32.Parse(x.DtFinal.Substring(8, 2))) : data))
                .ForMember(src => src.CelulasInteresse, opt => opt.MapFrom(x => x.CelulasInteresse != null ? x.CelulasInteresse.Split(',', StringSplitOptions.None).Select(y => Int32.Parse(y)) : new List<int>()));
        }
    }
}
