using System.Collections.Generic;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IReajusteContratoService
    {
        IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComFinalizacaoCadastradas();
        FiltroComPeriodo<ReajusteContratoGridDto> Filtrar(FiltroComPeriodo<ReajusteContratoGridDto> filtro);
        IEnumerable<KeyValueDto> ObterPrestadoresPorCelula(int idCelula);
        ResponseReajusteContrato ObterValoresAtuais(int idPrestador, bool filtrar);
        IEnumerable<LogReajusteContrato> ObterLogsPorId(int id);
        void ReajustarContrato(ReajusteContrato reajusteContrato);
        ValoresContratoPrestadorModel ConsultarReajuste(int id);
        void AprovarReajuste(int id);
        void NegarReajute(InativacaoFinalizacaoContratoDto inativacao);
        void EfetuarReajustesDeContratos();
    }
}
