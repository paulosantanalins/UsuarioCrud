using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoServico.Api.AutoMapper
{
    public class DtoToViewModelMappingProfile : Profile
    {
        public DtoToViewModelMappingProfile()
        {
            CreateMap<ServicoContratadoDto, ServicoContratadoVM>();
        }
    }
}
