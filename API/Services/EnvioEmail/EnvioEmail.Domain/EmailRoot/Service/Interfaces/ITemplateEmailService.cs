using EnvioEmail.Domain.EmailRoot.Dto;
using EnvioEmail.Domain.EmailRoot.Entity;
using Utils.Base;

namespace EnvioEmail.Domain.EmailRoot.Service.Interfaces
{
    public interface ITemplateEmailService
    {
        TemplateEmail BuscarPorIdComParametors(int id);
        TemplateEmail BuscarPorId(int id);
        void PersistirTemplateEmail(TemplateEmail templateEmail);
        FiltroGenericoDtoBase<TemplateEmailDto> Filtrar(FiltroGenericoDtoBase<TemplateEmailDto> filtro);
    }
}
