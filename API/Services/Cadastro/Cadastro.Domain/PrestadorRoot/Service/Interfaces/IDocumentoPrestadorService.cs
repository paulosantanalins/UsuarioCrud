using Cadastro.Domain.PrestadorRoot.Entity;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IDocumentoPrestadorService
    {
        void Persistir(DocumentoPrestador documentoPrestador, string arquivoBase64);
        void InativarDocumentoPrestador(int id);
        void UploadDocumentoParaMinIO(string nomeAnexo, string caminhoDocumento, string arquivoBase64);
        IEnumerable<DocumentoPrestador> BuscarTodosDocumentosPrestador(int idPrestador);        
    }
}
