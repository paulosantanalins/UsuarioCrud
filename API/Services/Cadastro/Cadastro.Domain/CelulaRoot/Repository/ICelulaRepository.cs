using Cadastro.Domain.CelulaRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace ControleAcesso.Domain.ControleAcessoRoot.Repository
{
    public interface ICelulaRepository : IBaseRepository<Celula>
    {
        List<Celula> BuscarTodosAtivasComInclude();
        List<Celula> BuscarCelulasGerente(List<int> celulasId);
        List<Celula> BuscarCelulasDiretor(List<int> celulasId);
        Celula BuscarPorIdAtivasComInclude(int idCelula);
    }
}
