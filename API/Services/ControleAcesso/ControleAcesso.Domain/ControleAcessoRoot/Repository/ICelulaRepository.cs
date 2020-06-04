using ControleAcesso.Domain.ControleAcessoRoot.Dto;
using ControleAcesso.Domain.ControleAcessoRoot.Entity;
using ControleAcesso.Domain.Interfaces;
using Logger.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface ICelulaRepository : IBaseRepository<Celula>
    {
        List<Celula> BuscarTodosAtivasComInclude();
        List<Celula> BuscarTodosComInclude();
        FiltroGenericoDto<CelulaDto> FiltrarCelula(FiltroGenericoDto<CelulaDto> filtro);
        Task<List<Auditoria>> BuscarListaDeAuditoriaDeCelula(int idCelula);
        string ObterEmailGerenteServico(int idCelula);
        bool ValidarExisteCelula(int numero);
        Celula BuscarComIncludes(int idCelula);
    }
}
