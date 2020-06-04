using Cadastro.Domain.FilialRoot.Dto;
using System.Collections.Generic;

namespace Cadastro.Domain.FilialRoot.Service.Interfaces
{
    public interface IFilialService
    {
        IEnumerable<FilialRmDto> BuscarNoRm(int idColigada);
        FilialRmDto BuscarFilialNoRm(int idFilial, int idColigada);
    }
}
