using AutoMapper;
using EnvioEmail.Api.ViewModels;
using EnvioEmail.Domain.EmailRoot.Entity;

namespace EnvioEmail.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<EmailVM, Email>();
            CreateMap<TemplateEmailVM, TemplateEmail>();
            CreateMap<ParametroTemplateVM, ParametroTemplate>();
            CreateMap<ValorParametroEmailVM, ValorParametroEmail>();
        }
    }
}
