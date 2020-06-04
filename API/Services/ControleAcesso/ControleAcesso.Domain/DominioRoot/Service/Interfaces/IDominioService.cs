using ControleAcesso.Domain.SharedRoot;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleAcesso.Domain.DominioRoot.Service.Interfaces
{
    public interface IDominioService
    {
        ICollection<ComboDefaultDto> BuscarItens(string tipoDominio);
        ICollection<ComboDefaultDto> BuscarTodosItens(string tipoDominio);
        int ObterIdDominio(string tipoDominio, string valorDominio);
        int? ObterIdDominio(int idValor, string tipoDominio);
    }
}
