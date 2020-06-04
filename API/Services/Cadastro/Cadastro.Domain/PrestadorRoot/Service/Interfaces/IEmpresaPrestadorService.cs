using Cadastro.Domain.PrestadorRoot.Entity;
using System.Data;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IEmpresaPrestadorService
    {
        Empresa Adicionar(EmpresaPrestador empresaPrestador);
        bool VerificaExisteEmpresaNoRm(string cnpj);
        bool VerificaExisteEmpresaNoEacesso(string cnpj, int idProfissional, IDbConnection dbConnection, IDbTransaction dbTransaction);
    }
}
