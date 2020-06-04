using Cadastro.Domain.DominioRoot.Entity;
using Cadastro.Domain.DominioRoot.ItensDominio;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Utils;
using Utils.Extensions;

namespace Cadastro.Domain.PrestadorRoot.Service.Interfaces
{
    public class DocumentoPrestadorService : IDocumentoPrestadorService
    {
        private readonly IDocumentoPrestadorRepository _documentoPrestadorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMinioService _minioService;
        private readonly IDominioService _dominioService;
        private readonly IConfiguration _configuration;
        private readonly IVariablesToken _variables;

        public DocumentoPrestadorService(IDocumentoPrestadorRepository documentoPrestadorRepository,
            IUnitOfWork unitOfWork,
            IMinioService minioService,            
            IDominioService dominioService,
            IConfiguration configuration, IVariablesToken variables)
        {
            _documentoPrestadorRepository = documentoPrestadorRepository;
            _unitOfWork = unitOfWork;
            _minioService = minioService;
            _dominioService = dominioService;
            _configuration = configuration;
            _variables = variables;
        }
        public void Persistir(DocumentoPrestador documentoPrestador, string arquivoBase64)
        {
            if (!String.IsNullOrEmpty(arquivoBase64))
            {
                var tipoDocumento = documentoPrestador.NomeAnexo.Split('.')[documentoPrestador.NomeAnexo.Split('.').Length - 1];
                documentoPrestador.CaminhoDocumento = $"{Guid.NewGuid().ToString()}.{tipoDocumento}";
                UploadDocumentoParaMinIO(documentoPrestador.NomeAnexo, documentoPrestador.CaminhoDocumento, arquivoBase64);
            }

            DomTipoDocumentoPrestador novoDominio = null;
            if (documentoPrestador.IdTipoDocumentoPrestador.Equals((int)SharedEnuns.TipoDocumentoPrestador.OUTROS) &&
                !String.IsNullOrEmpty(documentoPrestador.DescricaoTipoOutros))
            {
                novoDominio = new DomTipoDocumentoPrestador
                {
                    DescricaoValor = documentoPrestador.DescricaoTipoOutros,
                    Ativo = true,
                    ValorTipoDominio = SharedEnuns.TipoDocumentoPrestador.VL_TIPO_DOMINIO.GetDescription(),
                };
                _dominioService.AddTrackingNoDominio(novoDominio);
                documentoPrestador.IdTipoDocumentoPrestador = novoDominio.Id;
            }
            
            documentoPrestador.Usuario = _variables.UsuarioToken;
            documentoPrestador.DataAlteracao = DateTime.Now;

            _documentoPrestadorRepository.Adicionar(documentoPrestador);
            _unitOfWork.Commit();            
        }

        public void UploadDocumentoParaMinIO(string nomeAnexo, string caminhoDocumento, string arquivoBase64)
        {            
            var result = _minioService.SalvarArquivo(arquivoBase64,
                caminhoDocumento,
                MimeMapping.MimeUtility.GetMimeMapping(nomeAnexo),
                _configuration["S3Bucket:documentosPrestadorBucketName"]).Result;
        }

        
        public IEnumerable<DocumentoPrestador> BuscarTodosDocumentosPrestador(int idPrestador)
        {
            var contratos = _documentoPrestadorRepository.Buscar(x => x.IdPrestador == idPrestador && !x.Inativo);
            return contratos as List<DocumentoPrestador>;
        }

        public void CadastrarTipoOutrosSeNaoExistir(Dominio dominio)
        {
            _dominioService.Persistir(dominio);
        }

        public void InativarDocumentoPrestador(int id)
        {
            var documento = _documentoPrestadorRepository.BuscarPorId(id);
            documento.Inativo = true;
            _unitOfWork.Commit();
        }
    }
}
