using GestaoServico.Domain.GestaoServicoRoot.Entity;
using GestaoServico.Domain.GestaoVinculoClienteRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Domain.GestaoServicoRoot.Repository
{
    public interface IServiceRepository
    {
        Task<TipoServico> ObterTipoServicoPorSigla(string sigla);
        Task<List<Servico>> ObterTodosServicos();
        Task PersistirServico(Servico servico);
        Task<VinculoClienteServico> VerificarExistenciaDeServicoPorIdSalesForce(string idSalesForce);
        Task<bool> ValidarTipoServico(string sigla);
        Task PersistirServicoPai(ServicoPai servicoPai);
    }
}
