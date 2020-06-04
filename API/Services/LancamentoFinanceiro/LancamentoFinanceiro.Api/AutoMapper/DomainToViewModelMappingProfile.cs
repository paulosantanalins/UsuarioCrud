using AutoMapper;
using LancamentoFinanceiro.Api.ViewModels;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;

namespace LancamentoFinanceiro.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<RootLancamentoFinanceiro, LancamentoFinanceiroVM>();
            CreateMap<ItemLancamentoFinanceiro, ItemLancamentoFinanceiroVM>();
        }
    }
}
