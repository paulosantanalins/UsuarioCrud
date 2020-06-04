using AutoMapper;
using Forecast.Api.ViewModels;
using Forecast.Domain.ForecastRoot;

namespace Forecast.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<ForecastET, ForecastVM>();

           
        }
    }
}
