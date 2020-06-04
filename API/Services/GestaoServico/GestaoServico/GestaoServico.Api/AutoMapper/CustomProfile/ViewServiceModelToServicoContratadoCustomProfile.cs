using AutoMapper;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.StfAnalitcsDW.Model;

namespace GestaoServico.Api.AutoMapper.CustomProfile
{
    public class ViewServiceModelToServicoContratadoCustomProfile : Profile
    {
        public ViewServiceModelToServicoContratadoCustomProfile()
        {
            CreateMap<ViewServicoModel, ServicoContratado>()
                .ForMember(dest => dest.DtFinal, opt => opt.MapFrom(src => src.DtFimVigencia ?? DateTime.Now))
                .ForMember(dest => dest.DtInicial, opt => opt.MapFrom(src => src.DtInicioVigencia))
                .ForMember(dest => dest.DescTipoCelula, opt => opt.MapFrom(src => src.DescTipoCelula == "COMERCIAL" ? "COM" : "TEC"))
                .ForMember(dest => dest.FlReembolso, opt => opt.MapFrom(src => src.FlReembolso == 1 ? true : false))
                .ForMember(dest => dest.QtdExtraReembolso, opt => opt.MapFrom(src => src.QtdExtraReembolso ?? 0))
                .ForMember(dest => dest.NmTipoReembolso, opt => opt.MapFrom(src => src.NmTipoReembolso == 0 ? null : src.NmTipoReembolso == 1 ? "Nota de serviço" : "Nota de débito"))
                .ForMember(dest => dest.FlHorasExtrasReembosaveis, opt => opt.MapFrom(src => src.FlHorasExtrasReembolsaveis == 1 ? true : false))
                .ForMember(dest => dest.VlRentabilidade, opt => opt.MapFrom(src => src.VlRentabilidadePrevista))
                .ForMember(dest => dest.FlReoneracao, opt => opt.MapFrom(src => src.FlReoneracao == 1 ? true : false))
                .ForMember(dest => dest.FlFaturaRecorrente, opt => opt.MapFrom(src => src.FlFaturaRecorrente == 1 ? true : false))
                .ForMember(dest => dest.IdProdutoRM, opt => opt.MapFrom(src => src.NrProdutoRM))
                .ForMember(dest => dest.FormaFaturamento, opt => opt.MapFrom(src => src.DescFormaFaturamento))
                .ForMember(dest => dest.IdContrato, opt => opt.Ignore())
                ;
        }
        //Markup -> tabela vinculo markup
        //IdServico -> tabela depara
        //public string NomeServico { get; set; }
        //public int IdCliente { get; set; }
        //public string IdContrato { get; set; }
        //public int? IdFilial { get; set; }
    }
}
