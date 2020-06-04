using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.NotaFiscalRoot.Entity;
using Cadastro.Domain.NotaFiscalRoot.Service.Interfaces;
using Cadastro.Domain.PluginRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Cadastro.Domain.SharedRoot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NotaFiscalController : Controller
    {
        private readonly INotaFiscalService _notaFiscalService;
        private readonly IPrestadorService _prestadorService;
        private readonly IMinioService _minioService;
        private readonly IHorasMesPrestadorService _horasMesPrestadorService;
        private readonly IPluginRMService _pluginRMService;
        private readonly IConfiguration _configuration;

        public NotaFiscalController(INotaFiscalService notaFiscalService,
            IPrestadorService prestadorService,
            IMinioService minioService,
            IHorasMesPrestadorService horasMesPrestadorService,
            IPluginRMService pluginRMService,
            IConfiguration configuration)
        {
            _notaFiscalService = notaFiscalService;
            _prestadorService = prestadorService;
            _minioService = minioService;
            _horasMesPrestadorService = horasMesPrestadorService;
            _pluginRMService = pluginRMService;
            _configuration = configuration;
        }

        [HttpGet("buscar-nf-prestador-por-idHorasMesPrestador/{id}")]
        public IActionResult NfPrestadorInfoPorIdHorasMesPrestador(int id)
        {
            var nfPrestadorInfo = _notaFiscalService.BuscarNfPrestadorInfoPorIdHorasMesPrestador(id);
            if (nfPrestadorInfo != null)
            {
                return Ok(nfPrestadorInfo);
            }

            return BadRequest();
        }

        [HttpGet("buscar-nf-prestador-por-token/{token}")]
        public IActionResult BuscarNfPrestadorPorToken(string token)
        {
            var resultBD = _notaFiscalService.BuscarNfPrestadorInfoPorToken(token);

            if (resultBD != null)
            {
                var resultVM = Mapper.Map<PrestadorEnvioNfVM>(resultBD);
                resultVM.ValorNf = _prestadorService.ObterValor(resultBD.HorasMesPrestador, false);
                return Ok(resultVM);
            }
            else
            {
                return Ok();
            }

        }

        [HttpGet("token-por-idHorasMesPrestador/{idHorasMesPrestador}")]
        public IActionResult TokenPorIdHorasMesPrestador(int idHorasMesPrestador)
        {
            var token = _notaFiscalService.BuscarTokenPorIdHorasMesPrestador(idHorasMesPrestador);
            return Ok(token);
        }

        [HttpGet("download-nf-prestador/{nomeArquivo}")]
        public async Task<IActionResult> DownloadNfPrestador(string nomeArquivo)
        {
            var arquivoByteArray = await _minioService.BuscarPdfByteArray(nomeArquivo, _configuration["S3Bucket:eacessoBucketName"]);
            string nomeDeRetornoDoArquivo = _notaFiscalService.MontarNomeArquivoPdf(nomeArquivo);
            return File(arquivoByteArray, MediaTypeNames.Application.Pdf, nomeDeRetornoDoArquivo + ".pdf");
        }

        [HttpPost("enviar-nf-prestador")]
        [RequestSizeLimit(7100000)]
        public async Task<IActionResult> EnviarNfPrestador([FromBody] PrestadorEnvioNfVM prestadorEnvioNfVM)
        {
            if (prestadorEnvioNfVM.CaminhoNf != null)
            {
                prestadorEnvioNfVM.Token = Guid.NewGuid().ToString();
                prestadorEnvioNfVM.CaminhoNf = prestadorEnvioNfVM.Token;
            }

            var arquivoFoiSalvo = await _minioService.SalvarPdf(prestadorEnvioNfVM.PdfBase64, prestadorEnvioNfVM.Token, _configuration["S3Bucket:eacessoBucketName"]);

            if (arquivoFoiSalvo)
            {
                var prestadorEnvioNF = Mapper.Map<PrestadorEnvioNf>(prestadorEnvioNfVM);

                _horasMesPrestadorService.DefinirSituacaoNfHorasMesPrestador(prestadorEnvioNF);

                var listHorasMes = _notaFiscalService.BuscarTodosPorIdHorasMesPrestador(prestadorEnvioNF.IdHorasMesPrestador);
                if (listHorasMes.Any(x => x.CaminhoNf == null))
                {
                    _pluginRMService.SolicitarPagamentoRM(prestadorEnvioNF.IdHorasMesPrestador);
                }

                _horasMesPrestadorService.Commit();
            }
            return Ok();
        }
    }
}
