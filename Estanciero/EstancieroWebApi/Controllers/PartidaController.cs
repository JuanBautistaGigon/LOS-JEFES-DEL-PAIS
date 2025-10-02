using Microsoft.AspNetCore.Mvc;
using EstancieroResponse;
using EstancieroService;
using EstancieroRequest;
namespace EstancieroWebApi.Controllers
{
    [ApiController]
    [Route("controller")]
    public class PartidaController : ControllerBase
    {
        PartidaService partidaService = new PartidaService();

        [HttpPost]
        public IActionResult DarDeAltaPartida([FromBody] PartidaRequest partidaRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultado = partidaService.CrearPartida(partidaRequest);
            return Ok(resultado.Mensaje);
        }

        [HttpPut("{numeroPartida}")]
        public IActionResult CambiarEstado(int numeroPartida, [FromBody] EstadoActualPartida estadoNuevo)
        {
            if (estadoNuevo == EstadoActualPartida.Finalizada || estadoNuevo == EstadoActualPartida.Suspendida)
            {
                var resultado = partidaService.FinalizarPartida(numeroPartida);
            }
            var resulatado = partidaService.CambiarEstadoPartida(numeroPartida, estadoNuevo);
            if (!resulatado.Exito)
            {
                return NotFound(resulatado.Mensaje);
            }
            return Ok(resulatado.Mensaje);
        }


        [HttpPut("{dniJugador}")]
        public IActionResult LanzarDado(int dniJugador, int casilleroActual)

        {
          int dado = partidaService.LanzarDado(dniJugador, casilleroActual);

            if (dado is not 0)
            {
                return Ok($"el numero de dado que salio es {dado}");
            }
            return BadRequest("ocurrio un error");
        }

        [HttpPut ("{dniPropietarioNuevo}")]
        public IActionResult ComprarPropiedad(int dniPropietarioNuevo, int casilleroActual)
        {
            partidaService.ComprarPropiedad(dniPropietarioNuevo, casilleroActual);

            return Ok("propiedad comprada con exito");
        }
    }
}
