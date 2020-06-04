using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoPortifolio;
using GestaoServico.Api.ViewModels.GestaoRespasse;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.GestaoServicoContratadoRoot.Entity;
using System;
using Utils.Relatorios.Models;

namespace GestaoServico.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CategoriaContabilVM, CategoriaContabil>();
            CreateMap<TipoServicoVM, TipoServico>();
            CreateMap<ClassificacaoContabilVM, ClassificacaoContabil>();
            CreateMap<PortfolioServicoVM, PortfolioServico>();
            CreateMap<SolicitarRepasseVM, Repasse>();
            CreateMap<EscopoVM, EscopoServico>();

            CreateMap<VinculoMarkupServicoContratado, VinculoMarkupModel>();
        }
    }
}
