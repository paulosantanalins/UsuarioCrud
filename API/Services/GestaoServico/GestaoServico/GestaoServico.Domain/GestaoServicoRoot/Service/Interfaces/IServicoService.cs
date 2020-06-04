using GestaoServico.Domain.GestaoServicoRoot.Entity;
using GestaoServico.Domain.GestaoVinculoClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Domain.GestaoServicoRoot.Service.Interfaces
{
    public interface IServicoService
    {
        Task PersistirService(Servico servico);
        Task CriarServicos(VinculoClienteServico vinculoClienteServico, string codContrato, string siglaTipoServico, int idCelula, string cnpjFilial, int idCliente, int? idCelulaDelivery);
    }
}
