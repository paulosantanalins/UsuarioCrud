using Cadastro.Domain.PrestadorRoot.Entity;
using System.Data;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IClienteServicoPrestadorService
    {
        ClienteServicoPrestador Adicionar(ClienteServicoPrestador clienteServicoPrestador);
        ClienteServicoPrestador Atualizar(ClienteServicoPrestador clienteServicoPrestador);
        ClienteServicoPrestador BuscarPorId(int id);
        void Inativar(int idClienteServicoPrestador);
        void AtualizarEAcesso(ClienteServicoPrestador clienteServicoPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction);
        void InserirClienteServicoPrestadorEAcesso(ClienteServicoPrestador clienteServicoPrestador, IDbConnection dbConnection, IDbTransaction dbTransaction);

    }
}
