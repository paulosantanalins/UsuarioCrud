using AutoMapper;
using ControleAcesso.Api.ViewModels.ControleAcesso;
using ControleAcesso.Domain.BroadcastRoot.Dto;
using ControleAcesso.Domain.BroadcastRoot.Entity;
using ControleAcesso.Domain.BroadcastRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ControleAcesso.Api.Controllers
{
    [Route("api/[controller]")]
    public class BroadcastController : ControllerBase
    {
        private readonly IBroadcastService _broadcastService;

        public BroadcastController(IBroadcastService broadcastService) { _broadcastService = broadcastService; }


        [HttpGet]
        public IActionResult ObterBroadcasts(string usuario, string valorParaFiltrar)
        {          
            var broadcasts = _broadcastService.ObterBroadcastsDoUsuario(usuario, valorParaFiltrar);
            return Ok(broadcasts);
        }


        [HttpGet("broadcasts-nao-excluidos")]
        public IActionResult ObterTodosBroadcastsNaoExcluidos(string usuario)
        {
            var broadcasts = _broadcastService.ObterTodosBroadcastsNaoExcluidos(usuario);
            return Ok(broadcasts);
        }

        [HttpPut("marcar-broadcast-como-lido")]
        public IActionResult MarcarBroadcastComoLido([FromBody] BroadcastVM broadcastVM)
        {
            var broadcast = Mapper.Map<Broadcast>(broadcastVM);
            var broadcastAtualizado = _broadcastService.MarcarBroadcastComoLido(broadcast);
            return Ok(broadcastAtualizado);
        }

        [HttpPut("marcar-broadcast-como-excluido")]
        public IActionResult MarcarBroadcastComoExcluido([FromBody] BroadcastVM broadcastVM)
        {
            var broadcast = Mapper.Map<Broadcast>(broadcastVM);
            var broadcastAtualizado = _broadcastService.MarcarBroadcastComoExcluido(broadcast);
            return Ok(broadcastAtualizado);
        }

        [HttpPut("criar-broadcasts-abertura-periodo-repasse")]
        public IActionResult CriarBroadcastParaAberturaDePeriodoRepasse([FromBody]PeriodoRepasseDto periodoRepasseDto)
        {
            _broadcastService.CriarBroadcastsParaAberturaPeriodoRepasse(periodoRepasseDto);
            return Ok((new { notifications = "", success = true }));
        }

        [HttpPost("criar-broadcasts-reajuste-contrato/{funcionalidadeEnvio}")]
        public IActionResult CriarBroadcastsReajusteDeContrato([FromRoute] string funcionalidadeEnvio)
        {
            _broadcastService.CriarBroadcastsParaReajusteDeContrato(funcionalidadeEnvio);
            return Ok(new {notifications = "", success = true});
        }

        [HttpPut("criar-broadcasts-aprovacao-horas")]
        public IActionResult CriarBroadcastParaAprovacaoHoras([FromBody]List<BroadcastAprovacaoHorasDto> aprovacaoDto)
        {
            _broadcastService.CriarBroadcastsParaAprovacaoHoras(aprovacaoDto);
            return Ok((new { notifications = "", success = true }));
        }
    }
}
