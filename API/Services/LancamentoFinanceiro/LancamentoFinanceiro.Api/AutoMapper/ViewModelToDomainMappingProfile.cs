using AutoMapper;
using LancamentoFinanceiro.Api.ViewModels;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using System;

namespace LancamentoFinanceiro.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<LancamentoFinanceiroVM, RootLancamentoFinanceiro>()
                .ForMember(dest => dest.LgUsuario, opt => opt.MapFrom(src => src.LgUsuario.Length > 30 ? src.LgUsuario.Substring(0, 29) : src.LgUsuario))
                .ForMember(dest => dest.DescricaoOrigemLancamento, opt => opt.MapFrom(src => src.DescricaoOrigemLancamento.Length > 2 ? src.DescricaoOrigemLancamento.Substring(0, 2) : src.DescricaoOrigemLancamento))
                .ForMember(dest => dest.DescricaoTipoLancamento, opt => opt.MapFrom(src => src.DescricaoTipoLancamento.Length > 1 ? src.DescricaoTipoLancamento.Substring(0, 1) : src.DescricaoTipoLancamento))
                .ForMember(dest => dest.CodigoColigada, opt => opt.MapFrom(src => src.CodigoColigada.Length > 1 ? src.CodigoColigada.Substring(0, 9) : src.CodigoColigada));

            CreateMap<ItemLancamentoFinanceiroVM, ItemLancamentoFinanceiro>()
                .ForMember(dest => dest.VlLancamento, opt => opt.MapFrom(src => Math.Round(src.VlLancamento,2)))
                .ForMember(dest => dest.LgUsuario, opt => opt.MapFrom(src => src.LgUsuario.Length > 30 ? src.LgUsuario.Substring(0,29) : src.LgUsuario))
                .ForMember(dest => dest.VlDesc, opt => opt.MapFrom(src => src.VlDesc != null ? Math.Round(src.VlDesc.Value, 2) : 0))
                .ForMember(dest => dest.VlInc, opt => opt.MapFrom(src => src.VlInc != null ? Math.Round(src.VlInc.Value, 2) : 0));
        }
    }
}
