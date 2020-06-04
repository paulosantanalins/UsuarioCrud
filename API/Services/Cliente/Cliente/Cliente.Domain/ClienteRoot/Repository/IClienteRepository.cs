using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using Utils.Base;

namespace Cliente.Domain.ClienteRoot.Repository
{
    public interface IClienteRepository : IBaseRepository<ClienteET>
    {
        int? ObterClientePorIdSalesForce(string idSalesForce);
        // bool Validar(ClienteET clienteET);
        FiltroGenericoDto<ClienteET> Filtrar(FiltroGenericoDto<ClienteET> filtro);
        int VerificarIdCliente();
        Task<MultiselectDto> PopularComboClienteRepasse(int id);
    }
}
