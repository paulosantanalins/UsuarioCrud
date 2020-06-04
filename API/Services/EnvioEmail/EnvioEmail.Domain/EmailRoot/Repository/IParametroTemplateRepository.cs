using EnvioEmail.Domain.EmailRoot.Entity;

namespace EnvioEmail.Domain.EmailRoot.Repository
{
    public interface IParametroTemplateRepository : IBaseRepository<ParametroTemplate>
    {
        int? ObterIdPeloNome(string nomeParametro, int idTemplate);
    }
}
