using AutoMapper;
using Cadastro.Domain.PrestadorRoot.Dto;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using Utils.Base;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TransferenciaCltPjController : Controller
    {
        private readonly ITransferenciaCltPjService _transferenciaCltPjService;

        public TransferenciaCltPjController(
            ITransferenciaCltPjService transferenciaCltPjService)
        {
            _transferenciaCltPjService = transferenciaCltPjService;
        }

        [HttpPost("filtrar")]
        public IActionResult Filtrar([FromBody] FiltroGenericoDtoBase<TransferenciaCltPjDto> filtro)
        {
            var resultBD = _transferenciaCltPjService.Filtrar(filtro);
            return Ok(resultBD);
        }

        [HttpPost]
        public IActionResult Adicionar([FromBody] DadosCltEacessoDto dadosCltEacesso)
        {
            try
            {
                var mensagem = _transferenciaCltPjService.Adicionar(dadosCltEacesso);
                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
