using GestaoServico.Domain.GestaoPortifolioRoot.DTO;
using GestaoServico.Domain.GestaoPortifolioRoot.Entity;
using GestaoServico.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Utils.Base;

namespace GestaoServico.Domain.GestaoPortifolioRoot.Repository
{
    public interface IEscopoServicoRepository : IBaseRepository<EscopoServico>
    {
        bool VerificarExistenciaEscopoServicoIgual(EscopoServico escopoServico);
        FiltroGenericoDto<GridEscopoDTO> Filtrar(FiltroGenericoDto<GridEscopoDTO> filtro);
        IEnumerable<MultiselectDto> ObterAtivos();
        bool VerificarExclusaoValida(int idEscopo);
        IEnumerable<MultiselectDto> ObterAtivosPorPortfolio(int idPortfolio);
    }
}
