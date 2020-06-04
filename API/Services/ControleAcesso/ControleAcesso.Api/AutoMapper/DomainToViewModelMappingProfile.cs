using AutoMapper;
using ControleAcesso.Api.ViewModels;
using ControleAcesso.Api.ViewModels.ControleAcesso;
using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.DominioRoot.ItensDominio;
using System;

namespace ControleAcesso.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Funcionalidade, FuncionalidadeVM>();
            CreateMap<Celula, CelulaVM>();
            CreateMap<Broadcast, BroadcastVM>();
            CreateMap<VisualizacaoCelulaDto, CelulaVM>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.DescCelula.Split("|", StringSplitOptions.None)[0]))
                .ForMember(x => x.DescCelula, opt => opt.MapFrom(x => x.DescCelula.Split("| ", StringSplitOptions.None)[1]));
            CreateMap<Grupo, GrupoVM>()
            .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.DescGrupo));
            CreateMap<VinculoTipoCelulaTipoContabil, VinculoTipoCelulaTipoContabilVM>()
                .ForMember(x => x.tipoContabil, opt => opt.MapFrom(x => x.TipoContabil));
            CreateMap<DomTipoContabil, TipoCelulaVM>()
                .ForMember(x => x.Descricao, opt => opt.MapFrom(x => x.DescricaoValor));


        }
    }
}
