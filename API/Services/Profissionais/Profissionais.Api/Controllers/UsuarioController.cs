using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UsuarioApi.Domain.DominioRoot.Entity;
using UsuarioApi.Domain.DominioRoot.Repository;
using UsuarioApi.Domain.DominioRoot.Service.Interfaces;

namespace UsuarioApi.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioController(
            IUsuarioService usuarioService,
            IUsuarioRepository usuarioRepository,
            IMapper mapper
          )
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        [HttpGet("buscar-usuarios")]
        public IActionResult BuscarTodos()
        {
            var usuario = _usuarioService.BuscarTodos();
            return Ok(usuario);
        }

        [HttpGet("buscar-por-id/{id}")] 
        public IActionResult BuscarTodos([FromRoute]int id)
        {
            var usuario = _usuarioService.BuscarPorId(id);
            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Persistir([FromBody]Usuario usuario)
        {
            _usuarioService.Persistir(usuario);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Editar([FromBody]Usuario objUsuario, [FromRoute]int id)
        {
            var usuario = _usuarioService.BuscarPorId(id);

            usuario.Nome = objUsuario.Nome;
            usuario.Sobrenome = objUsuario.Sobrenome;
            usuario.Escolaridade = objUsuario.Escolaridade;
            usuario.Email = objUsuario.Email;
            usuario.DataNasc = objUsuario.DataNasc;


            _usuarioService.Editar(usuario);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var usuario = _usuarioService.BuscarPorId(id);

            if(usuario != null)
                _usuarioService.Delete(usuario);

            return Ok();
        }
    }
}
