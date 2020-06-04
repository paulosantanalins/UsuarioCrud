using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.DominioRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Utils.Extensions;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DocumentoPrestadorController : Controller
    {
        private readonly IDocumentoPrestadorService _documentoPrestadorService;
        private readonly IMinioService _minioService;
        private readonly IDominioService _dominioService;
        private readonly IConfiguration _configuration;

        public DocumentoPrestadorController(IDocumentoPrestadorService documentoPrestadorService,
            IMinioService minioService,
            IDominioService dominioService,
            IConfiguration configuration)
        {
            _documentoPrestadorService = documentoPrestadorService;
            _minioService = minioService;
            _dominioService = dominioService;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] DocumentoPrestadorVM documentoPrestadorVM)
        {
            var documentoPrestador = Mapper.Map<DocumentoPrestador>(documentoPrestadorVM);
            _documentoPrestadorService.Persistir(documentoPrestador,
                documentoPrestadorVM.ArquivoBase64 ?? null);

            return Ok();
        }

        [HttpPut("inativar-documento-prestador")]
        public IActionResult Inativar([FromBody] int idDocumento)
        {
            _documentoPrestadorService.InativarDocumentoPrestador(idDocumento);
            return Ok();
        }


        [HttpGet("todos-por-idPrestador")]
        public IActionResult ObterTodosPorIdPrestador(int idPrestador)
        {
            var documentos = _documentoPrestadorService.BuscarTodosDocumentosPrestador(idPrestador);
            var tiposDeDocumentoPrestador = _dominioService.BuscarDominios(SharedEnuns.TipoDocumentoPrestador.VL_TIPO_DOMINIO.GetDescription());

            var documentosVM = documentos.Select(x =>
            new DocumentoPrestadorVM
            {
                Id = x.Id,
                IdPrestador = x.IdPrestador,
                CaminhoDocumento = x.CaminhoDocumento,
                DescricaoTipoOutros = x.DescricaoTipoOutros,
                NomeAnexo = x.NomeAnexo,
                Tipo = tiposDeDocumentoPrestador?.FirstOrDefault(d => d.Id.Equals(x.IdTipoDocumentoPrestador))?.DescricaoValor ?? "",
                IdTipoDocumentoPrestador = x.IdTipoDocumentoPrestador,
                DataAlteracao = x.DataAlteracao,
                Usuario = x.Usuario
            });

            return Ok(documentosVM);
        }

        [HttpGet("download-documento-prestador")]
        public async Task<IActionResult> DownloadContratoPrestador(string caminhoContrato, string nomeAnexo)
        {
            var arquivoByteArray = await _minioService.BuscarPdfByteArray(caminhoContrato,
                _configuration["S3Bucket:documentosPrestadorBucketName"]);
            return File(arquivoByteArray, MediaTypeNames.Application.Pdf, nomeAnexo);
        }
    }
}
