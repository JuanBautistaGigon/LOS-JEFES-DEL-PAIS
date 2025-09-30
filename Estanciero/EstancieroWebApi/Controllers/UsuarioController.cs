using EstancieroRequest;
using EstancieroResponse;
using EstancieroService;
using Microsoft.AspNetCore.Mvc;

namespace EstancieroWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private UsuarioService usuarioService;
        public UsuarioController()
        {
            usuarioService = new UsuarioService();
        }
        [HttpGet]
        public IActionResult ObtenerUsuarios()
        {
            var usuarios = usuarioService.ObtenerUsuarios();
            if (usuarios.Count() >= 1 && ModelState.IsValid)
            {
                return Ok(usuarios);
            }
            else
            {
                return NotFound("No se encontraron usuarios");
            }
        }
        [HttpPost]
        public IActionResult AgregarUsuario([FromBody] UsuarioRequest usuarioRequest)
        {
            var resultado = usuarioService.CrearUsuario(usuarioRequest);
            if (resultado.tipoStatusCode == EstancieroEntities.TipoStatusCode.Ok)
            {
                return Ok(resultado.Mensaje);
            }
            else
            {
                return BadRequest("No se pudo crear el usuario");
            }
        }
    }
}
