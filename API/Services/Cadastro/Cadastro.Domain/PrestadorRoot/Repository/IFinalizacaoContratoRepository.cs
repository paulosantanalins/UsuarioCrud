using System.Collections.Generic;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface IFinalizacaoContratoRepository : IBaseRepository<FinalizacaoContrato>
    {
        IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastrada();
        FiltroComPeriodo<FinalizarContratoGridDto> Filtrar(FiltroComPeriodo<FinalizarContratoGridDto> filtro);
        IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula, bool filtrar);
        FinalizacaoContratoDto ConsultarFinalizacao(int id);
        void InativarFinalizacao(FinalizacaoContrato finalizacaoContrato, string motivo);
        void AdicionarComLog(FinalizacaoContrato finalizacaoContrato);
        void UpdateComLog(FinalizacaoContrato finalizacaoContrato);
        IEnumerable<LogFinalizacaoContrato> ObterLogsPorId(int id);
        IEnumerable<FinalizacaoContrato> ObterFinalizacoesParaJob();
    }
}
