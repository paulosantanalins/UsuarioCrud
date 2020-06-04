
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepasseEAcesso.Domain.RepasseRoot.Service.Interfaces;

namespace RepasseEAcesso.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigracaoController : ControllerBase
    {
        private readonly IRepasseService _repasseService;
        private readonly IMapper _mapper;

        public MigracaoController(
            IRepasseService repasseService,
            IMapper mapper)
        {
            _mapper = mapper;
            _repasseService = repasseService;
        }


        [HttpGet("realizar-migracao/repasse-eacesso/{senha}")]
        public IActionResult RealizarMigracaoRepasseEacesso([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _repasseService.RealizarMigracaoRepasseEacesso();
                return Ok("Migração repasse-eacesso Nível Um realizada com sucesso");
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("realizar-migracao/atualizacao-repasse-mae-eacesso/{senha}")]
        public IActionResult RealizarAtualizaçãoIdRepasseMaeEacesso([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                //  TODO TESTE IMPLEMENTACAO

                _repasseService.RealizarAtualizaçãoIdRepasseMaeEacesso();
                return Ok("Atualização na tabela RepasseNivelUm do campo IdRepasseMae com sucesso");
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
