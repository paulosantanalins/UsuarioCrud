using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using System;
using System.Collections.Generic;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public interface IContratoPrestadorService
    {
        ContratoPrestador Persistir(ContratoPrestador contratoPrestador);
        void InativarContratoPrestador(int idContratoPrestador);
        List<ContratoPrestador> ObterPorIdPrestador(int idPrestador);
        ContratoPrestador BuscarContratoPorPeriodo(int id, DateTime dataFim);        
        void UploadContratoParaMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64);
        ContratoPrestadorDto UploadArquivosDoContrato(ContratoPrestadorDto contrato);
    }
}
