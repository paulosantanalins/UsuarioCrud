using GestaoServico.Domain.GestaoPortifolioRoot.DTO;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Utils.Base;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Service.Interfaces
{
    public interface IEscopoServicoService
    {
        void PersistirEscopoServico(EscopoServico escopoServico);
        void ValidarEscopoValido(EscopoServico escopoServico);
        FiltroGenericoDto<GridEscopoDTO> Filtrar(FiltroGenericoDto<GridEscopoDTO> filtro);
        EscopoServico BuscarPorId(int idEscopo);
        void AlterarStatus(int idEscopo);
        IEnumerable<MultiselectDto> BuscarAtivos();
        IEnumerable<MultiselectDto> BuscarAtivosPorPortfolio(int idPortfolio);
        bool VerificarExclusaoValida(int id);
    }
}
