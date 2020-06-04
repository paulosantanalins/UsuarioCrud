using AutoMapper;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.MonitoramentoRoot.DTO;
using ControleAcesso.Domain.MonitoramentoRoot.Entity;
using ControleAcesso.Domain.SharedRoot;

namespace ControleAcesso.Api.AutoMapper
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<Celula, CelulaDto>();
            CreateMap<MonitoramentoBack, MonitoramentoBackDto>();
            CreateMap<MonitoramentoBack, ComboDefaultDto>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.Origem));
        }
    }
}
