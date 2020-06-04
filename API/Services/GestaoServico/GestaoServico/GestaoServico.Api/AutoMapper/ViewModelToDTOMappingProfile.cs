using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Api.ViewModels.GestaoRespasse;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Dto;
using GestaoServico.Infra.CrossCutting.IoC.DTO;
using System;
using Utils;
using Utils.Base;

namespace GestaoServico.Api.AutoMapper
{
    public class ViewModelToDTOMappingProfile : Profile
    {
        public ViewModelToDTOMappingProfile()
        {
            CreateMap<ServicoIntegracaoVM, ServicoIntegracaoDTO>();
            CreateMap<FiltroGenericoViewModel<ClassificacaoContabilVM>, FiltroGenericoDto<ClassificacaoContabil>>().ReverseMap();
            CreateMap<FiltroGenericoViewModel<CategoriaContabilVM>, FiltroGenericoDto<CategoriaContabil>>().ReverseMap();
            CreateMap<FiltroGenericoViewModel<TipoServico>, FiltroGenericoDto<TipoServico>>().ReverseMap();
            CreateMap<PortfolioServicoVM,PortfolioServicoDto>().ReverseMap();
            //CreateMap<FiltroGenericoViewModel<ContratoVM>, FiltroGenericoDto<ContratoDto>>().ReverseMap();
            CreateMap<FiltroGenericoViewModel<ClassificacaoContabilVM>,FiltroGenericoDto<ClassificacaoContabilDto>>().ReverseMap();
            CreateMap<FiltroGenericoViewModel<PortfolioServicoVM>, FiltroGenericoDto<PortfolioServicoDto>>().ReverseMap();
            CreateMap<FiltroGenericoViewModel<ServicoContratadoVM>, FiltroGenericoDto<ServicoContratadoDto>>().ReverseMap();
            CreateMap< FiltroGenericoDto<ServicoContratadoDto>, FiltroGenericoViewModel<ServicoContratadoVM>>();
            CreateMap<ContratoVM, ContratoDto>().ReverseMap();
            CreateMap<GridRepasseDto, GridRepasseVM>().ReverseMap();
            CreateMap<GridRepasseAprovarDto, GridRepasseAprovarVM>()
                .ForMember(dest => dest.Desabilita, opt => opt.MapFrom(x => x.Aprovado == "NA" ? false : true))
                .ReverseMap();
            CreateMap<FiltroGenericoDtoBase<GridRepasseDto>, FiltroGenericoDtoBase<GridRepasseVM>>().ReverseMap();
            CreateMap<GridServicoMigradoVM, GridServicoMigradoDTO>().ReverseMap();

            CreateMap<CelulaVM, CelulaVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.DescCelula.Contains("|") ? Int32.Parse(x.DescCelula.Split("|", StringSplitOptions.RemoveEmptyEntries)[0]) : x.Id));
        }
    }
}
