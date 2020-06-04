using Cadastro.Domain.PrestadorRoot.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IExtensaoContratoPrestadorService
    {
        void Persistir(ExtensaoContratoPrestador extensao, string arquivoBase64);
        void UploadExtensaoContratoParaMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64);
        void InativarExtensaoContratoPrestador(int id);
    }
}
