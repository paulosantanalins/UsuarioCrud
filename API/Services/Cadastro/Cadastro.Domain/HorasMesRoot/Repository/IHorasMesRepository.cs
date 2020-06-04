using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.HorasMesRoot.Repository
{
    public interface IHorasMesRepository : IBaseRepository<HorasMes>
    {
        List<HorasMes> BuscarPeriodos();
        HorasMes BuscarPeriodoVigente();
    }
}
