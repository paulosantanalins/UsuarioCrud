
using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Entity;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ContratoPrestadorController : Controller
    {
        private readonly IContratoPrestadorService _contratoPrestadorService;
        private readonly IMinioService _minioService;
        private readonly IConfiguration _configuration;

        public ContratoPrestadorController(IContratoPrestadorService contratoPrestadorService,
            IMinioService minioService, IConfiguration configuration)
        {
            _contratoPrestadorService = contratoPrestadorService;
            _minioService = minioService;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] ContratoPrestadorVM contratoPrestadorVM)
        {
            var contratoPrestadorDto = Mapper.Map<ContratoPrestadorDto>(contratoPrestadorVM);
            _contratoPrestadorService.UploadArquivosDoContrato(contratoPrestadorDto);

            var contratoPrestador = Mapper.Map<ContratoPrestador>(contratoPrestadorDto);
            contratoPrestador = _contratoPrestadorService.Persistir(contratoPrestador);

            return Ok();
        }

        [HttpGet("todos-por-idPrestador")]
        public IActionResult ObterTodosPorIdPrestador(int idPrestador)
        {
            var contratos = _contratoPrestadorService.ObterPorIdPrestador(idPrestador);
            var contratosVM = Mapper.Map<List<ContratoPrestadorVM>>(contratos);
            return Ok(contratosVM);
        }

        [HttpGet("download-contrato-prestador")]
        public async Task<IActionResult> DownloadContratoPrestador(string caminhoContrato, string nomeAnexo)
        {
            var arquivoByteArray = await _minioService.BuscarPdfByteArray(caminhoContrato,
                _configuration["S3Bucket:contratosPrestadorBucketName"]);
            return File(arquivoByteArray, MediaTypeNames.Application.Pdf, nomeAnexo);
        }

        [HttpPut("inativar-contrato-prestador")]
        public IActionResult InativarContratoPrestador([FromBody] int idContratoPrestador)
        {
            _contratoPrestadorService.InativarContratoPrestador(idContratoPrestador);
            return Ok();
        }
    }
}
