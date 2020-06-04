using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Configuration;
using System;
using Utils;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class ExtensaoContratoPrestadorService : IExtensaoContratoPrestadorService
    {
        private readonly IExtensaoContratoPrestadorRepository _extensaoContratoPrestadorRepository;
        private readonly IMinioService _minioService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVariablesToken _variables;

        public ExtensaoContratoPrestadorService(IExtensaoContratoPrestadorRepository extensaoContratoPrestadorRepository,
            IMinioService minioService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork, IVariablesToken variables)
        {
            _extensaoContratoPrestadorRepository = extensaoContratoPrestadorRepository;
            _minioService = minioService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _variables = variables;
        }

        public void Persistir(ExtensaoContratoPrestador extensao, string arquivoBase64)
        {
            if (!String.IsNullOrEmpty(arquivoBase64))
            {
                var tipoDocumento = extensao.NomeAnexo.Split('.')[extensao.NomeAnexo.Split('.').Length - 1];
                extensao.CaminhoContrato = $"{Guid.NewGuid().ToString()}.{tipoDocumento}";
                UploadExtensaoContratoParaMinIO(extensao.NomeAnexo, extensao.CaminhoContrato, arquivoBase64);
            }

            extensao.Usuario = _variables.UsuarioToken;
            extensao.DataAlteracao = DateTime.Now;

            _extensaoContratoPrestadorRepository.Adicionar(extensao);
            _unitOfWork.Commit();
        }

        public void UploadExtensaoContratoParaMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64)
        {
            var result = _minioService.SalvarArquivo(arquivoBase64,
                caminhoContrato,
                MimeMapping.MimeUtility.GetMimeMapping(nomeAnexo),
                _configuration["S3Bucket:contratosPrestadorBucketName"]).Result;
        }
      
        public void InativarExtensaoContratoPrestador(int id)
        {
            var extensao = _extensaoContratoPrestadorRepository.BuscarPorId(id);
            extensao.Inativo = true;            
            _unitOfWork.Commit();
        }
    }
}
