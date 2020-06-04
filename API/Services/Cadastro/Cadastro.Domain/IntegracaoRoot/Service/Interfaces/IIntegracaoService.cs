namespace Cadastro.Domain.IntegracaoRoot.Service.Interfaces
{
    public interface IIntegracaoService
    {
        bool Execute(string query);
        dynamic Select(string query);
        dynamic SelectEacesso(string query);
    }
}
