using AutoMapper;
using GestaoServico.Api.ViewModels.GestaoRespasse;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Dto;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Entity;
using GestaoServico.Domain.GestaoPacoteServicoRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Base;
using Utils.Connections;
using Utils.EacessoLegado.Service;

namespace GestaoServico.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Repasse")]
    public class RepasseController : Controller
    {
        protected readonly IRepasseService _repasseService;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public RepasseController(IRepasseService repasseService,
                                 IOptions<ConnectionStrings> connectionStrings)
        {
            _repasseService = repasseService;
            _connectionStrings = connectionStrings;
        }

        [HttpGet("filtrar")]
        public IActionResult FiltrarRepasse(FiltroGenericoViewModelBase<GridRepasseVM> filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroGenericoDtoBase<GridRepasseDto>>(filtroVM);
                var result = _repasseService.Filtrar(filtroDto);
                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("filtrar-aprovar")]
        public IActionResult FiltrarRepasseAprovar([FromQuery] FiltroAprovarRepasseVM filtroVM)
        {
            try
            {
                var filtroDto = Mapper.Map<FiltroAprovarRepasseDto>(filtroVM);
                var result = _repasseService.FiltrarAprovar(filtroDto);
                filtroVM = Mapper.Map<FiltroAprovarRepasseVM>(result);
                return Ok(filtroVM);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult PersistirRepasse([FromBody] SolicitarRepasseVM solicitarRepasse)
        {
            try
            {
                var repasse = Mapper.Map<Repasse>(solicitarRepasse);

                _repasseService.PersistirRepasse(repasse, solicitarRepasse.VezesRepitidas);
                return Ok((new { dados = "", notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult AtualizarRepasse([FromBody] SolicitarRepasseVM solicitarRepasse)
        {
            try
            {
                var repasse = Mapper.Map<Repasse>(solicitarRepasse);
                _repasseService.AtualizarRepasse(repasse);
                return Ok((new { dados = "", notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("editar-futuros")]
        public IActionResult AtualizarRepassesFuturos([FromBody] SolicitarRepasseVM solicitarRepasse)
        {
            try
            {
                var repasse = Mapper.Map<Repasse>(solicitarRepasse);
                _repasseService.AtualizarRepassesFuturos(repasse);
                return Ok((new { dados = "", notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}/{data}")]
        public IActionResult ObterRepassePorId(int id, string data)
        {
            try
            {
                var dataFormatada = DateTime.Parse(data);
                var profissionalEacessoService = new ProfissionalEacessoService(_connectionStrings.Value.EacessoConnection);
                var result = new SolicitarRepasseVM();
                //var resultRepasse = _repasseService.ObterRepassePorIdPorDataComDescricaoParcelada(id, dataFormatada);
                var resultRepasse = _repasseService.ObterRepassePorIdPorData(id, dataFormatada);
                result = Mapper.Map<SolicitarRepasseVM>(resultRepasse);
                result.VezesRepitidas = 1;//resultRepasse.RepasseFilhos.Count + 1;
                var resultProfissional = profissionalEacessoService.ObterProfissionalPorId(resultRepasse.IdProfissional);
                result.Profissional = resultProfissional;

                return Ok((new { dados = result, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete("CancelarRepasse/{id}")]
        public IActionResult CancelarRepasse(int id)
        {
            try
            {
                _repasseService.CancelarRepasse(id);
                return Ok((new { dados = "", notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}/cancelar-repasse-unico")]
        public IActionResult CancelarRepasseUnico(int id)
        {
            try
            {
                _repasseService.CancelarRepasseUnico(id);
                return Ok((new { dados = "", notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}/verificar-repasses-futuros")]
        public IActionResult VerificarExistenciaNotasFuturas(int id)
        {
            try
            {
                var resultRepasse = _repasseService.VerificarExistenciaParcelasFuturas(id);
                return Ok((new { dados = resultRepasse, notifications = "", success = true }));
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut("negar-repasse")]
        public IActionResult NegarRepasse([FromBody] GridRepasseAprovarVM repasse)
        {
            try
            {
                _repasseService.NegarRepasse(repasse.Id, repasse.Motivo);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("aprovar-repasse")]
        public IActionResult AprovarRepasse([FromBody] GridRepasseAprovarVM repasse)
        {
            try
            {
                _repasseService.AprovarRepasse(repasse.Id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("resetar-repasse")]
        public IActionResult ResetarRepasse([FromBody] GridRepasseAprovarVM repasse)
        {
            try
            {
                _repasseService.ResetarRepasse(repasse.Id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("aprovar-bloco-repasse")]
        public IActionResult AprovarBlocoRepasse([FromBody] List<GridRepasseAprovarVM> repasses)
        {
            try
            {
                var repassesDTO = Mapper.Map<List<GridRepasseAprovarDto>>(repasses);
                var problemas = Mapper.Map<List<GridRepasseAprovarVM>>(_repasseService.AprovarBlocoRepasse(repassesDTO));
                return Ok(problemas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{idServico}/repasses-origem-por-servico-contratado")]
        public IActionResult ObterRepassesPorServicoContratadoOrigem(int idServico)
        {
            try
            {
                var resultRepasse = _repasseService.ObterRepassesPorServicoContratadoOrigem(idServico);
                return Ok(resultRepasse);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idCelula}/repasses-origem-por-celula")]
        public IActionResult ObterRepassesPorCelulaOrigem(int idCelula)
        {
            try
            {
                var resultRepasse = _repasseService.ObterRepassesPorCelulaOrigem(idCelula);
                return Ok(resultRepasse);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idCelula}/{idCliente}/repasses-origem-por-celula-cliente")]
        public IActionResult ObterRepassesPorCelulaClienteOrigem(int idCelula, int idCliente)
        {
            try
            {
                var resultRepasse = _repasseService.ObterRepassesPorCelulaClienteOrigem(idCelula, idCliente);
                return Ok(resultRepasse);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idServico}/repasses-destino-por-servico-contratado")]
        public IActionResult ObterRepassesPorServicoContratadoDestino(int idServico)
        {
            try
            {
                var resultRepasse = _repasseService.ObterRepassesPorServicoContratadoDestino(idServico);
                return Ok(resultRepasse);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idCelula}/repasses-destino-por-celula")]
        public IActionResult ObterRepassesPorCelulaDestino(int idCelula)
        {
            try
            {
                var resultRepasse = _repasseService.ObterRepassesPorCelulaDestino(idCelula);
                return Ok(resultRepasse);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{idCelula}/{idCliente}/repasses-destino-por-celula-cliente")]
        public IActionResult ObterRepassesPorCelulaClienteDestino(int idCelula, int idCliente)
        {
            try
            {
                var resultRepasse = _repasseService.ObterRepassesPorCelulaClienteDestino(idCelula, idCliente);
                return Ok(resultRepasse);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("obter-repasses-destino-por-celulas-relatorio-diretoria")]
        public IActionResult ObterRepassesDestinoPorCelulaRelatorioRentabilidade(List<int> idsCelula)
        {
            try
            {
                var resultBD = _repasseService.ObterRepassesPorCelulasDestinoRelatorioDiretoria(idsCelula);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("obter-repasses-origem-por-celulas-relatorio-diretoria")]
        public IActionResult ObterRepassesOrigemPorCelulaRelatorioRentabilidade([FromQuery] List<int> idsCelula)
        {
            try
            {
                var resultBD = _repasseService.ObterRepassesPorCelulasDestinoRelatorioDiretoria(idsCelula);
                return Ok(resultBD);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
