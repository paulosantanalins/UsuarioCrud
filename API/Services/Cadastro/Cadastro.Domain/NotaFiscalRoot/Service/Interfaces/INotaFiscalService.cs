using Cadastro.Domain.NotaFiscalRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.NotaFiscalRoot.Service.Interfaces
{
    public interface INotaFiscalService
    {
        PrestadorEnvioNf BuscarNfPrestadorInfoPorToken(string token);
        void AtualizarNotaFiscalPrestador(PrestadorEnvioNf prestadorEnvioNf);
        PrestadorEnvioNf BuscarNfPrestadorInfoPorIdHorasMesPrestador(int id);
        string BuscarTokenPorIdHorasMesPrestador(int idHorasMesPrestador);
        string MontarNomeArquivoPdf(string nomeArquivo);
        List<PrestadorEnvioNf> BuscarTodosPorIdHorasMesPrestador(int idHorasMesPrestador);
    }
}
