using EstancieroRequest;
using EstancieroResponse;
using EstancieroService;
using Microsoft.AspNetCore.Mvc;

namespace EstancieroWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    
    public class UsuarioController : ControllerBase
    {
         private UsuarioService usuarioService;
         public UsuarioController()
         {
             usuarioService = new UsuarioService();
         }
         //Ver usuarios registrados
         [HttpGet]
        public IActionResult ObtenerUsuarios()
        {
            var usuarios = usuarioService.ObtenerUsuarios();
            var listaUsuariosSinEliminados = usuarios.Where(x => x.FechaEliminacion == null).ToList();
            if (listaUsuariosSinEliminados.Count() >= 1 && ModelState.IsValid)
            {
                return Ok(usuarios);
            }
            else
            {
                return NotFound("No se encontraron usuarios");
            }
        }
        //Registrar usuarios *
        [HttpPost]
         public IActionResult AgregarUsuario([FromBody] UsuarioRequest usuarioRequest)
         {
             var resultado = usuarioService.CrearUsuario(usuarioRequest);
             if (resultado.tipoStatusCode == TipoStatusCode.Ok)
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
             if (resultado.tipoStatusCode == TipoStatusCode.Ok)
             {
                 return Ok(resultado.Mensaje);
             }
             else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
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
             if (resultado.tipoStatusCode == TipoStatusCode.Ok)
             {
                 return Ok(resultado.Mensaje);
             }
             else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
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
