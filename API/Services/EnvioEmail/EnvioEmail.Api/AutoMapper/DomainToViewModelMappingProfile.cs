using AutoMapper;
using EnvioEmail.Api.ViewModels;
using EnvioEmail.Domain.EmailRoot.Entity;
using System.Net.Mail;

namespace EnvioEmail.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Email, EmailVM>();
            CreateMap<TemplateEmail, TemplateEmailVM>();
            CreateMap<ParametroTemplate, ParametroTemplateVM>().
                ForPath(x => x.IdParametro, opt => opt.MapFrom(x => x.Id));
        }
    }
}
