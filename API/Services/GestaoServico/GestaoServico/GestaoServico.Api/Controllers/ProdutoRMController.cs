using AutoMapper;
using GestaoServico.Api.ViewModels;
using GestaoServico.Api.ViewModels.GestaoServicoContratado;
using GestaoServico.Domain.Core.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProdutoRMController : Controller
    {
        private readonly NotificationHandler _notificationHandler;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public ProdutoRMController(NotificationHandler notificationHandler, IOptions<ConnectionStrings> connectionStrings)
        {
            _notificationHandler = notificationHandler;
            _connectionStrings = connectionStrings;
        }

        [HttpGet]
        public IActionResult Obtertodos()
        {
            try
            {
                var produtoEacessoService = new ProdutoRMEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = produtoEacessoService.ObterProdutos();
                var resultVM = Mapper.Map<IEnumerable<ProdutoVM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("PopularCombo/{filtro}")]
        public IActionResult ObterPopularCombo(string filtro)
        {
            try
            {
                var produtoEacessoService = new ProdutoRMEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = produtoEacessoService.ObterProdutosFiltrados(filtro);
                var resultVM = Mapper.Map<IEnumerable<ComboProdutoRM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("popular-combo-prestador/{idEmpresaGrupo}")]
        public IActionResult PopularComboPrestador(int idEmpresaGrupo)
        {
            try
            {
                var produtoEacessoService = new ProdutoRMEacessoService(_connectionStrings.Value.RMConnection);
                var resultBD = produtoEacessoService.PopularComboPrestador(idEmpresaGrupo);
                var resultVM = Mapper.Map<IEnumerable<ComboDefaultVM>>(resultBD);

                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("popular-combo-prestador-todos/{idEmpresaGrupo}")]
        public IActionResult PopularComboPrestadorTodos(int idEmpresaGrupo)
        {
            try
            {
                var produtoEacessoService = new ProdutoRMEacessoService(_connectionStrings.Value.RMConnection);
                var resultBD = produtoEacessoService.PopularComboPrestadorTodos(idEmpresaGrupo);
                var resultVM = Mapper.Map<IEnumerable<ComboDefaultVM>>(resultBD);

                return Ok(resultVM);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idProdutoRm}/{idColigada}")]
        public IActionResult BuscarPorId(int idProdutoRm, int idColigada)
        {
            try
            {
                var produtoEacessoService = new ProdutoRMEacessoService(_connectionStrings.Value.RMConnection);
                var produtoRM = produtoEacessoService.BuscarPorId(idProdutoRm, idColigada);

                if (produtoRM != null)
                {
                    return Ok(produtoRM.Descricao);
                }
                return Ok("Produto RM não encontrado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idEmpresa}/filtrar-por-empresa")]
        public IActionResult FiltrarPorEmpresa(int idEmpresa)
        {
            try
            {
                var produtoEacessoService = new ProdutoRMEacessoService(_connectionStrings.Value.EacessoConnection);
                var resultBD = produtoEacessoService.ObterProdutosPorEmpresa(idEmpresa);
                var resultVM = Mapper.Map<IEnumerable<ComboProdutoRM>>(resultBD);

                return Ok(new { dados = resultVM, notifications = "", success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }

}
