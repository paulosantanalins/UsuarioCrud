using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using Cadastro.Domain.DominioRoot.Dto;
using Utils;

namespace Cadastro.Domain.DominioRoot.Service.Interfaces
{
    public interface IDominioService
    {
        ICollection<ComboDefaultDto> BuscarItens(string tipoDominio);
        ICollection<ComboDefaultDto> BuscarTodosItens(string tipoDominio);
        void Persistir(Dominio dominio);
        void Editar(Dominio dominio);
        void MudarStatusDominio(int id);
        void AddTrackingNoDominio(Dominio dominio);
        List<Dominio> BuscarDominios(string tipoDominio);
        FiltroGenericoDto<DominioDto> FiltrarDominios(FiltroGenericoDto<DominioDto> filtro);
        DominioDto BuscarDominioPorId(int id);
        List<string> BuscarCombosGruposDominios();
        bool ValidarDominio(DominioDto dominioDto);
        ICollection<ComboDefaultDto> BuscarItensPropriedade(string tipoDominio);
    }
}
