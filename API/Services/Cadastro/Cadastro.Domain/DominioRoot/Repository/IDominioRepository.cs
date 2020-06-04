using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using Cadastro.Domain.DominioRoot.Dto;
using Utils;

namespace Cadastro.Domain.DominioRoot.Repository
{
    public interface IDominioRepository : IBaseRepository<Dominio>
    {
        int? ObterIdPeloCodValor(int idValor, string tipoDominio);
        List<Dominio> ObterPeloVlTipoDominio(string tipoDominio);
        FiltroGenericoDto<DominioDto> FiltrarDominios(FiltroGenericoDto<DominioDto> filtro);
        Dominio BuscarDominioPorId(int id);
        List<string> BuscarCombosGruposDominios();
        void MudarStatusDominio(Dominio dominio);
        bool ValidarDominio(DominioDto dominioDto);
        IEnumerable<ComboDefaultDto> BuscarDominiosPorTipo(string tipo, bool ativo);
        IEnumerable<ComboDefaultDto> BuscarDominiosPropriedade(string tipo, bool ativo);
    }
}
