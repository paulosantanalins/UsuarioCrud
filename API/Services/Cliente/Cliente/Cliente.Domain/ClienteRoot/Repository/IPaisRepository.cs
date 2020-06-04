using Cliente.Domain.ClienteRoot.Entity;
using Cliente.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cliente.Domain.ClienteRoot.Repository
{
    public interface IPaisRepository : IBaseRepository<Pais>
    {
        void SalvarPaises(List<Pais> paises);
    }
}
