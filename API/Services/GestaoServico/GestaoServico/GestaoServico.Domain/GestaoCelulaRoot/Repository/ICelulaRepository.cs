using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GestaoServico.Domain.GestaoCelulaRoot.Repository
{
    public interface ICelulaRepository
    {
        Task PersistirCelula();
    }
}
