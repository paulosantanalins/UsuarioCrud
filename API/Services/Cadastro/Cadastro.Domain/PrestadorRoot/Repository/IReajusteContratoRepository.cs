using System.Collections.Generic;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IReajusteContratoRepository : IBaseRepository<ReajusteContrato>
    {
        IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastrada();
        FiltroComPeriodo<ReajusteContratoGridDto> Filtrar(FiltroComPeriodo<ReajusteContratoGridDto> filtro);
        IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula);
        Prestador ObterPrestadorParaReajuste(int idPrestador, bool filtrar);
        IEnumerable<LogReajusteContrato> ObterLogsPorId(int id);
        void AdicionarComLog(ReajusteContrato reajusteContrato);
        void UpdateComLog(ReajusteContrato reajusteContrato, int acao);
        void InativarFinalizacao(ReajusteContrato reajusteContrato, string motivo);
        ValoresContratoPrestadorModel ConsultarReajuste(int id);
        IEnumerable<ReajusteContrato> ObterReajustesParaJob();
        int ObterIdFuncionalidade(string nomeFuncionalidade);
        IEnumerable<string> ObterLoginsComFuncionalidade(int idFuncionalidade, Prestador prestador);
        IEnumerable<string> ObterEmailGerenteCelula(int idFuncionalidade, Prestador prestador);
    }
}
