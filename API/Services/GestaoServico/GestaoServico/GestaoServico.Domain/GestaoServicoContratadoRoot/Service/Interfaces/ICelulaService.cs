using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces
{
    public interface ICelulaService
    {
        IEnumerable<Celula> ObterTodos();
    }
}
