using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.SharedRoot;
using System.Collections.Generic;
using Utils;

namespace Cadastro.Domain.PrestadorRoot.Repository
{
    public interface ITransferenciaPrestadorRepository : IBaseRepository<TransferenciaPrestador>
    {
        FiltroComPeriodo<TransferenciaPrestadorDto> FiltrarTransferencias(FiltroComPeriodo<TransferenciaPrestadorDto> filtro);
        IEnumerable<PeriodoTransferenciaPrestadorDto> BuscarPeriodosComTransferenciaCadastrada();

        bool ExisteTransferenciaAberta(int idPrestador);
        PrestadorParaTransferenciaDto ConsultarTransferencia(int idTransferencia);
    }
}
