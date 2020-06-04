using Cadastro.Domain.PrestadorRoot.Entity;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IEmpresaService
    {
        Empresa BuscarPorId(int id);
        Empresa BuscarPorCnpj(string cnpj);
        void AtualizarEmpresaPrestador(Empresa empresa);
        void SalvarEmpresaPrestador(Empresa empresa);
        void Inativar(int id);
        void AtualizarEmpresaDoPrestadorEAcesso(Empresa empresa, int idPrestadorEacesso);
        void AdicionarEmpresaDoPrestadorEAcesso(Empresa empresa, int idPrestadorEacesso, Prestador prestador);
        string ObterCodEmpresaRm(int idPrestador);
    }
}
