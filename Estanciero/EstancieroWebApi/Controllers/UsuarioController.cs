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
        [HttpPut]
        public IActionResult ModificarUsuario([FromBody] UsuarioRequest usuarioRequest)
        {
            var resultado = usuarioService.ModificarUsuario(usuarioRequest);
            if (resultado.tipoStatusCode == EstancieroEntities.TipoStatusCode.Ok)
            {
                return Ok(resultado.Mensaje);
            }
            else if (resultado.tipoStatusCode == EstancieroEntities.TipoStatusCode.NotFound)
            {
                return NotFound(resultado.Mensaje);
            }
            else
            {
                return BadRequest("No se pudo modificar el usuario");
            }
        }
        [HttpDelete]
        public IActionResult EliminarUsuario(int dni)
        {
            var resultado = usuarioService.EliminarUsuario(dni);
            if (resultado.tipoStatusCode == EstancieroEntities.TipoStatusCode.Ok)
            {
                return Ok(resultado.Mensaje);
            }
            else if (resultado.tipoStatusCode == EstancieroEntities.TipoStatusCode.NotFound)
            {
                return NotFound(resultado.Mensaje);
            }
            else
            {
                return BadRequest("No se pudo eliminar el usuario");
            }
        }
    }
}
