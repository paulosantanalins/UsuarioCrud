using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IContratoPrestadorRepository : IBaseRepository<ContratoPrestador>
    {

        ContratoPrestador BuscarContratoPorPeriodo(int id, DateTime? dataInicio);
        ContratoPrestador BuscarContratoNoPeriodoVigente(int idPrestador);
        IEnumerable<ContratoPrestador> BuscarComIncludes(int idPrestador);
        ContratoPrestador BuscarContratoComIncludes(int idContrato);
    }
}
