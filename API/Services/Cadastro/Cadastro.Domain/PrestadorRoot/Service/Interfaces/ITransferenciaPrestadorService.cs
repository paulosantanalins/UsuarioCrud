using System.Collections.Generic;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Utils;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface ITransferenciaPrestadorService
    {
        IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula);
        IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComTransferenciasCadastradas();
        ResponseTransferenciaPrestador ObterPrestadorParaTransferencia(int idPrestador, bool validar);
        string SolicitarTransferenciaPrestador(TransferenciaPrestador transferenciaPrestador);
        string AtualizarTransferenciaPrestador(TransferenciaPrestador transferenciaPrestador);
        FiltroComPeriodo<TransferenciaPrestadorDto> FiltrarTransferencia(FiltroComPeriodo<TransferenciaPrestadorDto> filtro);
        PrestadorParaTransferenciaDto ConsultarTransferencia(int idTransferencia);
        IEnumerable<LogTransferenciaPrestador> ObterLogsTransferencia(int idTransf);
        void AprovarTransferencia(AprovarTransferenciaPrestadorDto aprovacaoDto);
        void NegarTransferencia(NegarTransferenciaPrestadorDto negacaoDto);
        void EfetivarTransferenciasDePrestadoresJob();
    }
}
