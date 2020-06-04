using System.Collections.Generic;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IFinalizacaoContratoService
    {
        IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastradas();
        FiltroComPeriodo<FinalizarContratoGridDto> Filtrar(FiltroComPeriodo<FinalizarContratoGridDto> filtro);
        IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula, bool filtrar);
        void FinalizarContrato(FinalizacaoContrato finalizacaoContrato, FinalizacaoContratoDto finalizacaoContratoDto);
        FinalizacaoContratoDto ConsultarFinalizacao(int id);
        void InativarFinalizacao(InativacaoFinalizacaoContratoDto inativacaoFinalizacaoContratoDto);
        void EditarFinalizacao(FinalizacaoContrato finalizacaoContratoDto, bool finalizarImediatamente);
        IEnumerable<LogFinalizacaoContrato> ObterLogsPorId(int id);
        void EfetuarFinalizacoes();
    }
}
