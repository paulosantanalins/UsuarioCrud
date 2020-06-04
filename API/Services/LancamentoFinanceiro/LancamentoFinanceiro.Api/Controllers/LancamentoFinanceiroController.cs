using AutoMapper;
using LancamentoFinanceiro.Api.ViewModels;
using LancamentoFinanceiro.Domain.Core.Notifications;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Entity;
using LancamentoFinanceiro.Domain.LancamentoFinanceiroRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LancamentoFinanceiro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/LancamentoFinanceiro")]
    public class LancamentoFinanceiroController : Controller
    {
        private readonly ILancamentoFinanceiroService _lancamentoFinanceiroService;
        private readonly NotificationHandler _notificationHandler;

        public LancamentoFinanceiroController(
            ILancamentoFinanceiroService lancamentoFinanceiroService,
            NotificationHandler notificationHandler)
        {
            _lancamentoFinanceiroService = lancamentoFinanceiroService;
            _notificationHandler = notificationHandler;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var lancamentosBD = _lancamentoFinanceiroService.ObterTodos();
                var lancamentosVM = Mapper.Map<IEnumerable<LancamentoFinanceiroVM>>(lancamentosBD);
                return Ok(lancamentosVM);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("gerar-credito-debito")]
        public IActionResult GerarCreditoDebito ([FromBody] List<LancamentoFinanceiroVM> lancamentosVM)
        {
            var lancamentos = Mapper.Map<List<RootLancamentoFinanceiro>>(lancamentosVM);
            _lancamentoFinanceiroService.AdicionarRange(lancamentos);
            return Ok();
        }

        [HttpPost]
        public IActionResult PersistirLancamentoFinanceiro([FromBody] LancamentoFinanceiroVM lancamentosVM)
        {
            try
            {
                var lancamento = Mapper.Map<RootLancamentoFinanceiro>(lancamentosVM);
                _lancamentoFinanceiroService.Adicionar(lancamento);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{idRepasse}/remover-lancamentos-relacionados-repasse")]
        public IActionResult RemoverLancamentosRelacionadosRepasse(int idRepasse)
        {
            try
            {
                _lancamentoFinanceiroService.RemoverLancamentosFinanceirosPorRepasse(idRepasse);
                if (_notificationHandler.Mensagens.Any())
                {
                    return StatusCode(500);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
