using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using System.Linq;

namespace Cadastro.Domain.NotaFiscalRoot.Repository
{
    public interface IPrestadorEnvioNfRepository : IBaseRepository<PrestadorEnvioNf>
    {
        PrestadorEnvioNf BuscarInfoParaEnvioNfPrestadorPorToken(string token);
        PrestadorEnvioNf BuscarNfPrestadorInfoPorIdHorasMesPrestador(int id);        
        IQueryable<PrestadorEnvioNf> BuscarPorIdHorasMesPrestador(int idHorasMesPrestador);

    }
}
