using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Repository  
{
    public interface IValorPrestadorBeneficioRepository : IBaseRepository<ValorPrestadorBeneficio>
    {
        List<ValorPrestadorBeneficio> BuscarValoresPrestadorBeneficio(int idValorPrestador);
    }
}
