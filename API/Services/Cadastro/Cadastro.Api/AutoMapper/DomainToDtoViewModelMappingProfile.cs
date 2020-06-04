using AutoMapper;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Api.AutoMapper
{
    public class DomainToDtoViewModelMappingProfile : Profile
    {
        public DomainToDtoViewModelMappingProfile()
        {
            CreateMap<TransferenciaPrestador, TransferenciaPrestadorDto>()
             .ForPath(x => x.Prestador, opt => opt.MapFrom(x => x.Prestador.Pessoa.Nome))
             .ForPath(x => x.Celula, opt => opt.MapFrom(x => x.IdCelula))
             .ForPath(x => x.Status, opt => opt.MapFrom(x => x.Situacao));

        }
    }
}
