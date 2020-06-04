using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IHorasMesPrestadorRepository : IBaseRepository<HorasMesPrestador>
    {
        int BuscarPorIdHoraMes(int idHoraMes);
        HorasMesPrestador BuscarLancamentoParaPeriodoVigente(int idPrestador, int idHorasMes);
        HorasMesPrestador BuscarLancamentoParaPeriodoVigenteIdHoraMesPrestador(int idHoraMesPrestador);
        List<HorasMesPrestador> BuscarAprovacoesPendentes(int idHoraMes);
        HorasMesPrestador BuscarPorIdComIncludes(int id);
        List<HorasMesPrestador> BuscarLancamentosAprovados(int idHorasMes);
        List<HorasMesPrestador> BuscarLancamentosComPagamentoPendente(int idHorasMes);
        List<HorasMesPrestador> BuscarLancamentosComPagamentoSolicitado(int idHorasMes);
    }
}
