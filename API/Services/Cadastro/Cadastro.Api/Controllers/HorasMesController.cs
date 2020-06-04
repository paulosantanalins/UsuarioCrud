using AutoMapper;
using Cadastro.Api.ViewModels;
using Cadastro.Domain.EmpresaGrupoRoot.Service.Interfaces;
using Cadastro.Domain.HorasMesRoot.Entity;
using Cadastro.Domain.HorasMesRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class HorasMesController : Controller
    {
        protected readonly IHorasMesService _horasMesService;
        protected readonly IHorasMesPrestadorService _horasMesPrestadorService;

        public HorasMesController(
            IHorasMesService horasMesService,
            IHorasMesPrestadorService horasMesPrestadorService)
        {
            _horasMesService = horasMesService;
            _horasMesPrestadorService = horasMesPrestadorService;
        }


        [HttpGet("periodos")]
        public IActionResult BuscarPeriodos()
        {
            var resultBD = _horasMesService.BuscarPeriodos();
            var resultVM = Mapper.Map<IEnumerable<HorasMesVM>>(resultBD);

            var periodoAberto = resultVM.FirstOrDefault();
            if (periodoAberto != null)
            {
                periodoAberto.HabilitarEdicao = _horasMesPrestadorService.BuscarPorIdHoraMes(periodoAberto.Id) == 0;
            }
            return Ok(resultVM);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarHorasMes(int id)
        {
            var resultBD = _horasMesService.BuscarPorId(id);
            var resultVM = Mapper.Map<HorasMesVM>(resultBD);
            return Ok(resultVM);
        }

        [HttpGet("{id}/valida-campo-hora-mes")]
        public IActionResult ValidaCampoHoraMes(int id)
        {
            var resultBD = _horasMesPrestadorService.BuscarPorIdHoraMes(id);
            return Ok((new { dados = resultBD, notifications = "", success = true }));
        }

        [HttpPost]
        public IActionResult Persistir([FromBody] HorasMesVM horasMesVM)
        {
            var horasMes = Mapper.Map<HorasMes>(horasMesVM);
            if (horasMes.Id == 0)
            {
                horasMes.Ativo = true;
                _horasMesService.SalvarHorasMes(horasMes);
            }
            else
            {
                _horasMesService.AtualizarHorasMes(horasMes);
            }

            return Ok(true);
        }


        [HttpPost("Inativar")]
        public IActionResult Inativar([FromBody] int id)
        {
            _horasMesService.Inativar(id);
            return Ok(new { dados = "", notifications = "", success = true });
        }


        [HttpGet("emailAprovador")]
        public IActionResult emailAprovador()
        {
            _horasMesPrestadorService.EnviarEmailParaAprovacaoHoras();
            return Ok();
        }
    }
}
