using Cadastro.Domain.NatcorpRoot.Dto;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.NatcorpRoot.Repository
{
    public interface IProfissionalNatcorpRepository
    {        
        IEnumerable<ProfissionalNatcorpDto> BuscarProfissionaisNatcorpParaMigracao(DateTime? data);
    }
}
