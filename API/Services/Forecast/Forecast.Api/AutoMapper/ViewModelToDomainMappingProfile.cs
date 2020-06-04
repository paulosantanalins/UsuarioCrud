using AutoMapper;
using Forecast.Api.ViewModels;
using Forecast.Domain.ForecastRoot;
using System;
using Utils;

namespace Forecast.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ForecastVM, ForecastET>()
                .ForPath(x => x.ValorForecast.IdCelula, opt => opt.MapFrom(x => x.IdCelula))
                .ForPath(x => x.ValorForecast.IdCliente, opt => opt.MapFrom(x => x.IdCliente))
                .ForPath(x => x.ValorForecast.IdServico, opt => opt.MapFrom(x => x.IdServico))
                .ForPath(x => x.ValorForecast.NrAno, opt => opt.MapFrom(x => x.NrAno))
                .ForPath(x => x.ValorForecast.ValorAjuste, opt => opt.MapFrom(x => x.ValorAjuste))
                .ForPath(x => x.ValorForecast.ValorJaneiro, opt => opt.MapFrom(x => x.ValorJaneiro))
                .ForPath(x => x.ValorForecast.ValorFevereiro, opt => opt.MapFrom(x => x.ValorFevereiro))
                .ForPath(x => x.ValorForecast.ValorMarco, opt => opt.MapFrom(x => x.ValorMarco))
                .ForPath(x => x.ValorForecast.ValorAbril, opt => opt.MapFrom(x => x.ValorAbril))
                .ForPath(x => x.ValorForecast.ValorMaio, opt => opt.MapFrom(x => x.ValorMaio))
                .ForPath(x => x.ValorForecast.ValorJunho, opt => opt.MapFrom(x => x.ValorJunho))
                .ForPath(x => x.ValorForecast.ValorJulho, opt => opt.MapFrom(x => x.ValorJulho))
                .ForPath(x => x.ValorForecast.ValorAgosto, opt => opt.MapFrom(x => x.ValorAgosto))
                .ForPath(x => x.ValorForecast.ValorSetembro, opt => opt.MapFrom(x => x.ValorSetembro))
                .ForPath(x => x.ValorForecast.ValorOutubro, opt => opt.MapFrom(x => x.ValorOutubro))
                .ForPath(x => x.ValorForecast.ValorNovembro, opt => opt.MapFrom(x => x.ValorNovembro))
                .ForPath(x => x.ValorForecast.ValorDezembro, opt => opt.MapFrom(x => x.ValorDezembro))
                .ForPath(x => x.ValorForecast.VlPercentual, opt => opt.MapFrom(x => x.VlPercentual))
                //.ForPath(x => x.ValorForecast.Usuario, opt => opt.MapFrom(x => Variables.UsuarioToken))
                .ForPath(x => x.ValorForecast.DataAlteracao, opt => opt.MapFrom(x => DateTime.Now));
        }
    }
}
