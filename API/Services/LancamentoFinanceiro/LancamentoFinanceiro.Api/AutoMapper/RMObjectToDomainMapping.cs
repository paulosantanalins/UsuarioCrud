using AutoMapper;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.RM.Models;

namespace LancamentoFinanceiro.Api.AutoMapper
{
    public class RMObjectToDomainMapping : Profile
    {
        public RMObjectToDomainMapping()
        {
            CreateMap<LancamentoFinanceiroRMDTO, RootLancamentoFinanceiro>()
                .ForMember(src => src.DescricaoOrigemLancamento, opt => opt.MapFrom(x => x.DescricaoOrigemLancamento))
                .ForMember(src => src.DtLancamento, opt => opt.MapFrom(x => DateTime.Now))
                .ForMember(src => src.DtBaixa, opt => opt.MapFrom(x => x.DtBaixa.Date))
                .ForMember(src => src.DescricaoTipoLancamento, opt => opt.MapFrom(x => x.DescricaoTipoLancamento))
                //.ForMember(src => src.IdLan, opt => opt.MapFrom(x => x.IdServicoContratado))
                .ForMember(src => src.CodigoColigada, opt => opt.MapFrom(x => x.CodigoColigada))
                .ForMember(x => x.ItensLancamentoFinanceiro, opt => opt.MapFrom(z => new List<ItemLancamentoFinanceiro>{ new ItemLancamentoFinanceiro
                                                                                                                {
                                                                                                                    IdServicoContratado = z.IdServicoContratado,
                                                                                                                    VlLancamento = z.VlLancamento,
                                                                                                                    CodigoCusto = z.CodigoCusto
                                                                                                                } }));

        }
    }
}