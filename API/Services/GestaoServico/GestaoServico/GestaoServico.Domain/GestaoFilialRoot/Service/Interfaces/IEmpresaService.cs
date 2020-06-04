using GestaoServico.Domain.GestaoFilialRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoFilialRoot.Service.Interfaces
{
    public interface IEmpresaService
    {
        IEnumerable<Empresa> ObterTodos();
    }
}
