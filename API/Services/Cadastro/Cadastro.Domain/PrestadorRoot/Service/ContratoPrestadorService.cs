using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Repository;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Cadastro.Domain.PrestadorRoot.Service
{
    public class ContratoPrestadorService : IContratoPrestadorService
    {
        private readonly IContratoPrestadorRepository _contratoPrestadorRepository;
        private readonly IExtensaoContratoPrestadorService _extensaoContratoPrestadorService;
        private readonly IExtensaoContratoPrestadorRepository _extensaoContratoPrestadorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMinioService _minioService;
        private readonly IConfiguration _configuration;
        private readonly IVariablesToken _variables;

        public ContratoPrestadorService(IContratoPrestadorRepository contratoPrestadorRepository,
            IExtensaoContratoPrestadorService extensaoContratoPrestadorService,
            IExtensaoContratoPrestadorRepository extensaoContratoPrestadorRepository,
            IUnitOfWork unitOfWork,
            IMinioService minioService,
            IConfiguration configuration, IVariablesToken variables)
        {
            _contratoPrestadorRepository = contratoPrestadorRepository;
            _extensaoContratoPrestadorService = extensaoContratoPrestadorService;
            _extensaoContratoPrestadorRepository = extensaoContratoPrestadorRepository;
            _unitOfWork = unitOfWork;
            _minioService = minioService;
            _configuration = configuration;
            _variables = variables;
        }
        public ContratoPrestador Persistir(ContratoPrestador contratoPrestador)
        {         
            contratoPrestador.Usuario = _variables.UsuarioToken;
            contratoPrestador.DataAlteracao = DateTime.Now;

            _contratoPrestadorRepository.Adicionar(contratoPrestador);
            _unitOfWork.Commit();

            return contratoPrestador;
        }

        public List<ContratoPrestador> ObterPorIdPrestador(int idPrestador)
        {
            var contratos = _contratoPrestadorRepository.BuscarComIncludes(idPrestador).OrderBy(x => x.DataFim).ToList();
            foreach (var contrato in contratos)
            {
                contrato.ExtensoesContratoPrestador = contrato.ExtensoesContratoPrestador.Where(x => !x.Inativo).ToList();
                contrato?.ExtensoesContratoPrestador.OrderBy(x => x.Tipo);
            }
            return contratos.ToList();
        }

        public ContratoPrestador BuscarContratoPorPeriodo(int idPrestador, DateTime dataInicio)
        {
            var contratoPrestador = _contratoPrestadorRepository.BuscarContratoPorPeriodo(idPrestador, dataInicio);
            return contratoPrestador;
        }

        public ContratoPrestadorDto UploadArquivosDoContrato(ContratoPrestadorDto contrato)
        {
            if (!String.IsNullOrEmpty(contrato.ArquivoBase64))
            {
                var tipoDocumento = contrato.NomeAnexo.Split('.')[contrato.NomeAnexo.Split('.').Length - 1];
                contrato.CaminhoContrato = $"{Guid.NewGuid().ToString()}.{tipoDocumento}";
                UploadContratoParaMinIO(contrato.NomeAnexo, contrato.CaminhoContrato, contrato.ArquivoBase64);
            }

            if (contrato.ExtensoesContratoPrestador.Count > 0)
            {
                foreach (var extensao in contrato.ExtensoesContratoPrestador)
                {
                    if (!String.IsNullOrEmpty(extensao.ArquivoBase64))
                    {
                        var tipoDocumento = extensao.NomeAnexo.Split('.')[extensao.NomeAnexo.Split('.').Length - 1];
                        extensao.CaminhoContrato = $"{Guid.NewGuid().ToString()}.{tipoDocumento}";
                        _extensaoContratoPrestadorService.UploadExtensaoContratoParaMinIO(extensao.NomeAnexo, extensao.CaminhoContrato, extensao.ArquivoBase64);
                    }
                }
            }

            return contrato;
        }

        public void UploadContratoParaMinIO(string nomeAnexo, string caminhoContrato, string arquivoBase64)
        {
            var result = _minioService.SalvarArquivo(arquivoBase64,
                caminhoContrato,
                MimeMapping.MimeUtility.GetMimeMapping(nomeAnexo),
                _configuration["S3Bucket:contratosPrestadorBucketName"]).Result;
        }

        public void InativarContratoPrestador(int idContratoPrestador)
        {
            var contratoPrestador = _contratoPrestadorRepository.BuscarContratoComIncludes(idContratoPrestador);
            contratoPrestador.Inativo = true;
            foreach (var extensao in contratoPrestador.ExtensoesContratoPrestador)
            {
                extensao.Inativo = true;
                _extensaoContratoPrestadorRepository.Update(extensao);
            }
            _contratoPrestadorRepository.Update(contratoPrestador);
            _unitOfWork.Commit();
        }
    }
}
