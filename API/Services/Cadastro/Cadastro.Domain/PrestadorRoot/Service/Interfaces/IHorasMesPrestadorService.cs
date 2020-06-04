using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IHorasMesPrestadorService
    {
        int BuscarPorIdHoraMes(int id);
        void SalvarHoras(HorasMesPrestador prestadorHoras);
        void EnviarEmailParaAprovacaoHoras();
        void DefinirSituacaoNfHorasMesPrestador(PrestadorEnvioNf prestadorEnvioNF);
        void AprovarPagamento(int id);
        void NegarPagamento(int id, string motivo);
        HorasMesPrestador BuscarPorId(int id);
        HorasMesPrestador BuscarPorIdComInclude(int id);
        List<HorasMesPrestador> BuscarLancamentosAprovados();
        List<HorasMesPrestador> BuscarLancamentosComPagamentoPendente(int idHorasMes);
        List<HorasMesPrestador> BuscarLancamentosComPagamentoSolicitado();
        List<HorasMesPrestador> BuscarHorasMesPrestadorPorPrestador(int idPrestador);
        void Commit();
        bool VerificarIntegracaoPrestadorRm(HorasMesPrestador horasMesPrestador);

     
    }
}
