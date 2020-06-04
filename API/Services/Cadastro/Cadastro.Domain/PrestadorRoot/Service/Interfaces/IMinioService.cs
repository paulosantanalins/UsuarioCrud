using System.Threading.Tasks;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IMinioService
    {
        Task<bool> SalvarPdf(string arquivoBase64, string nomeArquivo, string bucketName);
        Task<byte[]> BuscarPdfByteArray(string nomeArquivo, string bucketName);
        Task<bool> SalvarArquivo(string arquivoBase64, string nomeArquivo, string mimeType, string bucketName);
    }
}
