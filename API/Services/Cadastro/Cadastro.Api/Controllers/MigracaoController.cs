using Cadastro.Domain.PessoaRoot.Service.Interfaces;
using Cadastro.Domain.PrestadorRoot.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadastro.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Migracao")]
    public class MigracaoController : Controller
    {
        private readonly IPrestadorService _prestadorService;
        private readonly IPessoaService _pessoaService;

        public MigracaoController(
            IPrestadorService prestadorService,
            IPessoaService pessoaService)
        {
            _prestadorService = prestadorService;
            _pessoaService = pessoaService;
        }

        [HttpGet("clt-pj/{idProfissionalEacesso}")]
        public IActionResult RealizarMigracaoCltPj([FromRoute] int idProfissionalEacesso)
        {
            _prestadorService.RealizarMigracaoCltPj(idProfissionalEacesso);
            return Ok();
        }

        [HttpGet("realizar-migracao/prestador/{senha}")]
        public IActionResult RealizarMigracaoPrestador([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.RealizarMigracao(null);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/empresa-prestador/{senha}")]
        public IActionResult AtualizarMigracaoPrestador([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracao(null);
                return Ok();    
            }
            else if (senha == 0)
            {
                return Unauthorized();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet("atualizar-migracao/inativacao-prestador/{senha}")]
        public IActionResult AtualizarMigracaoInativacaoPrestador([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoInativacaoPrestador(null);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/cliente-servico/{senha}")]
        public IActionResult AtualizarMigracaoClienteServico([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoClienteServico(null);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/remuneracao-prestador/{senha}")]
        public IActionResult AtualizarMigracaoRemuneracaoPrestador([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoRemuneracaoPrestador(null);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/beneficio/{senha}")]
        public IActionResult AtualizarMigracaoBeneficio([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoBeneficio(null);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/observacao-prestador/{senha}")]
        public IActionResult AtualizarMigracaoObservacaoPrestador([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoObservacaoPrestador(null);
                return Ok("Migração realizada com sucesso");
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/prestador-pessoa/{senha}")]
        public IActionResult AtualizarMigracaoPrestadorPessoa([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoPrestadorPessoa();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/prestador-inicio-clt/{senha}")]
        public IActionResult AtualizarMigracaoPrestadorInicioCLT([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoPrestadorInicioCLT();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("atualizar-migracao/socios-empresa-prestador/{senha}")]
        public IActionResult AtualizarMigracaoSociosEmpresaPrestador([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _prestadorService.AtualizarMigracaoSociosEmpresaPrestador();
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet("atualizar-migracao/profissionais-natcorp/{senha}")]
        public IActionResult AtualizarMigracaoProfissionaisNatcorp([FromRoute] int senha)
        {
            if (senha == 1234)
            {
                _pessoaService.RealizarMigracaoProfissionaisNatcorp(null);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
