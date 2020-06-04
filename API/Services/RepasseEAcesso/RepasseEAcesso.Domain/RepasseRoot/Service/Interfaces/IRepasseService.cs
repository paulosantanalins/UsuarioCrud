using RepasseEAcesso.Domain.RepasseRoot.Entity;
using System.Collections.Generic;

namespace RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces
{
    public interface IRepasseService
    {
        Repasse BuscarPorId(int id);
        void RealizarMigracaoRepasseEacesso();
        void RealizarAtualizaçãoIdRepasseMaeEacesso();
    }
}
