using EnvioEmail.Domain.EmailRoot.Dto;
using EnvioEmail.Domain.EmailRoot.Entity;
using System.Collections.Generic;
using Utils.Base;

namespace EnvioEmail.Domain.EmailRoot.Repository
{
    public interface ITemplateEmailRepository : IBaseRepository<TemplateEmail>
    {
        TemplateEmail BuscarPorNome(string nome);
        void UpdateComParametro(TemplateEmail entity);
        TemplateEmail BuscarPorIdComParametros(int id);
        void AdicionarTemplateEmail(TemplateEmail entity);
        FiltroGenericoDtoBase<TemplateEmailDto> FiltrarTemplatesEmail(FiltroGenericoDtoBase<TemplateEmailDto> filtro);
    }
}
