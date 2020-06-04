using RepasseEAcesso.Domain.SharedRoot.DTO;
using System.Collections.Generic;

namespace RepasseEAcesso.Domain.DominioRoot.Service.Interface
{
    public interface IDominioService
    {
        ICollection<ComboDefaultDto> BuscarItens(string tipoDominio);

        string ObterDescricaoStatus(int IdStatusRepasse);
    }
}
