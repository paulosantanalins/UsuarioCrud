using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface ILogHorasMesPrestadorRepository : IBaseRepository<LogHorasMesPrestador>
    {
        List<LogHorasMesPrestador> BuscarLogsPorPrestador(HorasMesPrestador prestadorHoras);
    }
}
